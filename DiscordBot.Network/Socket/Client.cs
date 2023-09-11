// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

namespace DiscordBot.Network.Socket;

using DiscordBot.BinaryUtils;

public class Client {

    protected readonly System.Net.Sockets.Socket socket;

    public Client(System.Net.Sockets.Socket socket) {
        this.socket = socket;
    }

    public void Write(BinaryStream data) {
        this.Write(data.GetBuffer());
    }

    public void Write(byte[] data) {
        BinaryStream stream = new();
        stream.PutInt((uint)data.Length);
        stream.Put(data);
        this.socket.Send(stream.GetBuffer(), System.Net.Sockets.SocketFlags.None);
    }

    public Task<int> WriteAsync(BinaryStream data) {
        return this.WriteAsync(data.GetBuffer());
    }

    public Task<int> WriteAsync(byte[] data) {
        //Console.WriteLine("Writing " + Convert.ToHexString(data));
        BinaryStream stream = new();
        stream.PutInt((uint)data.Length);
        stream.Put(data);
        return this.socket.SendAsync(stream.GetBuffer(), System.Net.Sockets.SocketFlags.None);
    }

    public BinaryStream Read() {
        byte[] bytes = new byte[4];
        int received;
        try {
            received = this.socket.Receive(bytes, System.Net.Sockets.SocketFlags.None);
        }catch(Exception) {
            throw new Exception("Failed to receive data from socket.");
        }
        if(received != 4) {
            throw new Exception("4 bytes expected, received: " + received.ToString());
        }
        BinaryStream init = new(bytes);
        uint size = init.GetInt();
        byte[] data = new byte[size];
        try {
            received = this.socket.Receive(data, System.Net.Sockets.SocketFlags.None);
        }catch(Exception) {
            throw new Exception("Failed to receive data from socket.");
        }
        if(received != size) {
            throw new Exception(size.ToString() + " bytes expected, received: " + received.ToString());
        }
        return new BinaryStream(data);
    }

    public void Close() {
        this.socket.Close();
    }
}

