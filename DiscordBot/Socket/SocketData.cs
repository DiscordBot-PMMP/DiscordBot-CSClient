// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using System.Net;

namespace DiscordBot.Socket;

public class SocketData {

    public readonly IPAddress ipAddress;
    public readonly ushort port;
    public readonly IPEndPoint ipEndPoint;

    private readonly List<List<byte>> outbound = new();
    private readonly List<List<byte>> inbound = new();

    public SocketData(string ipAddress = "0.0.0.0", ushort port = 22222) {
        this.ipAddress = IPAddress.Parse(ipAddress);
        this.port = port;
        this.ipEndPoint = new(this.ipAddress, this.port);
    }

    public List<byte>? ReadInbound() {
        if(this.inbound.Count == 0) {
            return null;
        }
        var c = this.inbound.First();
        this.inbound.RemoveAt(0);
        return c;
    }

    public List<byte>? ReadOutbound() {
        if(this.outbound.Count == 0) {
            return null;
        }
        var c = this.outbound.First();
        this.outbound.RemoveAt(0);
        return c;
    }

    public void WriteInbound(List<byte> data) {
        this.inbound.Add(data);
    }

    public void WriteOutbound(List<byte> data) {
        this.outbound.Add(data);
    }
}
