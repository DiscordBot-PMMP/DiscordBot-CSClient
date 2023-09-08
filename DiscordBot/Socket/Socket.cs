// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using System.Net.Sockets;
using DiscordBot.BinaryUtils;

namespace DiscordBot.Socket;

public class Socket {

    public readonly SocketData socketData;
    protected System.Net.Sockets.Socket socket;

    private Client? client = null;
    private CancellationTokenSource? taskToken = null;
    private int? heartbeat = null;

    public Socket(SocketData socketData) {
        this.socketData = socketData;
        this.socket = new(this.socketData.ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        this.socket.Bind(this.socketData.ipEndPoint);
    }

    private void Listen() {
        this.socket.Listen();
        Console.WriteLine("Socket listening on " + this.socketData.ipAddress + ":" + this.socketData.port);
    }

    private void AcceptClient() {
        if(this.client != null) {
            throw new Exception("Client already connected, disconnect before re-accepting new client.");
        }
        Console.WriteLine("Waiting for new client connection...");
        try {
            this.client = new(this.socket.Accept());
        }catch(Exception) {
            throw new Exception("Failed to accept new connection.");
        }
        Console.WriteLine("Client connected.");
    }

    private void DisconnectClient(bool close = true) {
        if(close) {
            Console.WriteLine("Disconnecting client.");
            this.client?.Close();
        }
        this.taskToken?.Cancel();
        this.heartbeat = null;
        this.client = null;
    }

    public void Start() {
        Thread.CurrentThread.Name = "SocketThread";
        Console.WriteLine("Socket Thread started.");
        this.Listen();
        this.BaseLoop();
    }

    public void Stop() {
        this.DisconnectClient();
        this.client?.Close();
        this.socket.Close();
    }

    private void Loop() {
        this.taskToken = new();

        // Read / Write from socketData queues (is this necessary, can we directly call write/read ??)
        _ = Task.Run(() => this.ReadLoop(this.taskToken.Token), this.taskToken.Token);
        _ = Task.Run(() => this.WriteLoop(this.taskToken.Token), this.taskToken.Token);

        this.HeartbeatLoop(this.taskToken.Token);
    }

    private void ConnectionLoop() {
        while(this.client == null) {
            try {
                this.AcceptClient();
            }catch(Exception) {
                throw;
            }
        }

        Console.WriteLine("Waiting for connect packet...");

        // Receive initial connect packet.
        BinaryStream stream;
        try {
            stream = this.client.Read();
        }catch(Exception) {
            Console.Error.WriteLine("Exception occured when reading from client.");
            this.DisconnectClient();
            return;
        }

        // --- Connect packet. ---

        ushort packetId = stream.GetShort();

        if(packetId != 100) {
            Console.Error.WriteLine("Expected Connect packet (100), received: " + packetId.ToString());
            this.DisconnectClient();
            return;
        }

        uint uid = stream.GetInt();
        byte version = stream.GetByte();
        uint magic = stream.GetInt();
        if(version != 2 || magic != 0x4A61786B) {
            Console.Error.WriteLine("Version/Magic does not match expected. (" + version.ToString() + ", " + magic.ToString("X2"));
            this.DisconnectClient();
            return;
        }

        Console.WriteLine($"Recieved connect packet ({uid}), version: {version} , magic: 0x" + magic.ToString("X2"));

        BinaryStream response = new();
        response.PutShort(100); //packet
        response.PutInt(0); //uid
        response.PutByte(2); //ver
        response.PutInt(0x4A61786B); //magic

        try {
            this.client.Write(response);
        }catch(Exception) {
            Console.Error.WriteLine("Failed to write connect packet to client.");
            this.DisconnectClient();
            return;
        }

        // Connected !
        Console.WriteLine("Client fully connected.");
    }

    private void BaseLoop() {
        bool exit = false;
        while(!exit) {
            while(this.client == null) {
                try{
                    this.ConnectionLoop();
                }catch(Exception e) {
                    Console.WriteLine(e.Message);
                    exit = true;
                    break;
                }
            }

            if(!exit) {
                this.Loop();
            }
        }
    }
    
    private void HeartbeatLoop(CancellationToken cancellationToken) {
        while(!cancellationToken.IsCancellationRequested && this.client != null) {
            if(this.heartbeat != null) {
                //TODO check.
            }
            BinaryStream packet = new();
            packet.PutShort(1); //PID
            packet.PutInt(0); //UID
            packet.PutInt((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            _ = this.client.WriteAsync(packet);
            Console.WriteLine("Writing hp");
            Thread.Sleep(1000);
        }
    }

    private void ReadLoop(CancellationToken cancellationToken) {
        Thread.CurrentThread.Name = "ReadThread";
        while (!cancellationToken.IsCancellationRequested && this.client != null) {
            try {
                //TODO Check Disconnect & Heartbeat packets.
                this.socketData.WriteInbound(this.client.Read());
            }catch(Exception) {
                this.DisconnectClient();
                break;
            }
        }
    }

    private void WriteLoop(CancellationToken cancellationToken) {
        Thread.CurrentThread.Name = "WriteThread";
        while(!cancellationToken.IsCancellationRequested && this.client != null) {
            BinaryStream? data = this.socketData.ReadOutbound();
            while(data != null){
                _ = this.client.WriteAsync(data);
                data = this.socketData.ReadOutbound();
            }
            Thread.Sleep(100);
        }
    }
}
