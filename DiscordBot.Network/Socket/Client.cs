// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

namespace DiscordBot.Network.Socket;

using DiscordBot.BinaryUtils;
using DiscordBot.Network.Packets;

public class Client {

    protected readonly System.Net.Sockets.Socket socket;

    public Client(System.Net.Sockets.Socket socket) {
        this.socket = socket;
    }

    public void WritePacket(IOutboundPacket pk) {
        BinaryStream bs = new();
        bs.PutShort((ushort)(pk.GetType().GetProperty("Id")?.GetGetMethod()?.Invoke(null, null) ?? throw new Exception("Failed to get ID from packet " + pk.ToString())));
        bs.Put(pk.BinarySerialize().GetBuffer());
        this.Write(bs.GetBuffer());
    }

    public void Write(byte[] data) {
        BinaryStream stream = new();
        stream.PutInt((uint)data.Length);
        stream.Put(data);
        //Console.WriteLine("Writing " + Convert.ToHexString(stream.GetBuffer()));
        this.socket.Send(stream.GetBuffer(), System.Net.Sockets.SocketFlags.None);
    }

    public Task<int> WriteAsyncPacket(IOutboundPacket pk) {
        BinaryStream bs = new();
        bs.PutShort((ushort)(pk.GetType().GetProperty("Id")?.GetGetMethod()?.Invoke(null, null) ?? throw new Exception("Failed to get ID from packet " + pk.ToString())));
        bs.Put(pk.BinarySerialize().GetBuffer());
        return this.WriteAsync(bs.GetBuffer());
    }

    public Task<int> WriteAsync(byte[] data) {
        BinaryStream stream = new();
        stream.PutInt((uint)data.Length);
        stream.Put(data);
        //Console.WriteLine("Writing " + Convert.ToHexString(stream.GetBuffer()));
        return this.socket.SendAsync(stream.GetBuffer(), System.Net.Sockets.SocketFlags.None);
    }

    public Packet ReadPacket(bool handle = true) {
        byte[] bytes = new byte[4];
        int received;
        try {
            received = this.socket.Receive(bytes, System.Net.Sockets.SocketFlags.None);
        } catch(Exception) {
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
        } catch(Exception) {
            throw new Exception("Failed to receive data from socket.");
        }
        if(received != size) {
            throw new Exception(size.ToString() + " bytes expected, received: " + received.ToString());
        }
        BinaryStream bs = new(data);
        Packet pk = NetworkAPI.GetPacket(bs);
        if(handle) {
            pk.Handle();
        }
        return pk;
    }

    public void Close() {
        this.socket.Close();
    }
}

