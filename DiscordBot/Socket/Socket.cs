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

    protected readonly SocketData socketData;
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
        this.client = new(this.socket.Accept());
    }

    private void DisconnectClient(bool close = true) {
        if(close) {
            this.client?.Close();
        }
        this.taskToken?.Cancel();
        this.heartbeat = null;
        this.client = null;
    }

    public void Start() {
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
            this.AcceptClient();
        }

        Console.WriteLine("Client connected, waiting for initial packet...");

        // Receive initial connect packet.
        BinaryStream stream = this.client.Read();

        // --- Connect packet. ---

        ushort packetId = stream.GetShort();

        if(packetId != 100) {
            Console.Error.WriteLine("Expected Connect packet (100), received: " + packetId.ToString());
            this.DisconnectClient();
            this.ConnectionLoop();
            return;
        }

        uint uid = stream.GetInt();
        byte version = stream.GetByte();
        uint magic = stream.GetInt();
        if(version != 2 || magic != 0x4A61786B) {
            Console.Error.WriteLine("Version/Magic does not match expected. (" + version.ToString() + ", " + magic.ToString("X2"));
            this.DisconnectClient();
            this.ConnectionLoop();
            return;
        }

        Console.WriteLine($"Recieved connect packet ({uid}), version: {version} , magic: 0x" + magic.ToString("X2"));

        BinaryStream response = new();
        response.PutShort(100); //packet
        response.PutInt(0); //uid
        response.PutByte(2); //ver
        response.PutInt(0x4A61786B); //magic

        this.client.Write(response);

        // Connected !
        Console.WriteLine("Connected.");
    }

    private void BaseLoop() {
        while(this.client == null) {
            this.ConnectionLoop();
        }

        this.Loop();
    }
    
    private void HeartbeatLoop(CancellationToken cancellationToken) {
        while(!cancellationToken.IsCancellationRequested && this.client != null) {
            if(this.heartbeat != null) {
                //check.
            }
            BinaryStream packet = new();
            packet.PutShort(1); //PID
            packet.PutInt(0); //UID
            packet.PutInt((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            _ = this.client.WriteAsync(packet);
            Thread.Sleep(1000);
        }
    }

    private void ReadLoop(CancellationToken cancellationToken) {
        Thread.CurrentThread.Name = "ReadThread";
        while (!cancellationToken.IsCancellationRequested && this.client != null) {
            this.socketData.WriteInbound(this.client.Read());
            Thread.Sleep(100);
        }
    }

    private void WriteLoop(CancellationToken cancellationToken) {
        Thread.CurrentThread.Name = "WriteThread";
        while(!cancellationToken.IsCancellationRequested && this.client != null) {
            BinaryStream? data = this.socketData.ReadOutbound();
            if(data != null) {
                _ = this.client.WriteAsync(data);
            }
            Thread.Sleep(100);
        }
    }
}
