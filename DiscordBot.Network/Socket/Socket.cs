// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using System.Net.Sockets;
using DiscordBot.Network.Packets;
using DiscordBot.Network.Packets.External;
using DiscordBot.Network.Packets.Misc;

namespace DiscordBot.Network.Socket;

public class Socket {

    public readonly SocketData socketData;
    protected System.Net.Sockets.Socket socket;

    private Client? client = null;
    private CancellationTokenSource? taskToken = null;
    private uint? heartbeat = null;

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
        if(close && this.client != null) {
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
        this.RegisterInternalHandlers();
        this.BaseLoop();
    }

    public void Stop() {
        this.DisconnectClient();
        this.client?.Close();
        this.socket.Close();
    }

    private void RegisterInternalHandlers() {
        PacketHandler<Heartbeat>.AddHandler(Heartbeat.Id, new Action<Heartbeat>(this.HandleHeartbeat));
        PacketHandler<Disconnect>.AddHandler(Disconnect.Id, new Action<Disconnect>(this.HandleDisconnect));
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
        Connect packet;
        try {
            Packet pk = this.client.ReadPacket(false);
            if(pk is not Connect) {
                throw new Exception("Expecting Connect packet, but got " + pk.ToString());
            }
            packet = (Connect)pk;
        }catch(Exception) {
            Console.Error.WriteLine("Exception occured when reading from client.");
            this.DisconnectClient();
            return;
        }

        // --- Connect packet. ---

        byte version = packet.Version;
        uint magic = packet.Magic;
        if(version != 2 || magic != 0x4A61786B) {
            Console.Error.WriteLine("Version/Magic does not match expected. (" + version.ToString() + ", " + magic.ToString("X2"));
            this.DisconnectClient();
            return;
        }

        Console.WriteLine($"Recieved connect packet, version: {version} , magic: 0x" + magic.ToString("X2"));

        Connect response = new() {
            Version = 2,
            Magic = 0x4A61786B
        };

        try {
            this.client.WritePacket(response);
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

    private void HandleDisconnect(Disconnect pk) {
        Console.WriteLine("Disconnection request received: " + pk.Message);
        this.DisconnectClient();
    }

    private void HandleHeartbeat(Heartbeat pk) {
        this.heartbeat = pk.Timestamp;
    }

    private void HeartbeatLoop(CancellationToken cancellationToken) {
        while(!cancellationToken.IsCancellationRequested && this.client != null) {
            uint time = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();
            if(this.heartbeat != null) {
               if(time - this.heartbeat > 120) {
                    // No response received/parsed in 2 minutes.
                    Console.WriteLine("Client has not responded in 2 minutes, disconnecting.");
                    this.DisconnectClient();
                    return;
                }
            }
            _ = this.client.WriteAsyncPacket(new Heartbeat() { Timestamp = time });
            Thread.Sleep(1000);
        }
    }

    private void ReadLoop(CancellationToken cancellationToken) {
        Thread.CurrentThread.Name = "ReadThread";
        while (!cancellationToken.IsCancellationRequested && this.client != null) {
            try {
                this.client.ReadPacket();
            }catch(Exception) {
                this.DisconnectClient();
                break;
            }
        }
    }

    private void WriteLoop(CancellationToken cancellationToken) {
        Thread.CurrentThread.Name = "WriteThread";
        while(!cancellationToken.IsCancellationRequested && this.client != null) {
            Packet? data = this.socketData.ReadOutbound();
            while(data != null){
                _ = this.client.WriteAsyncPacket(data);
                data = this.socketData.ReadOutbound();
            }
            Thread.Sleep(100);
        }
    }
}
