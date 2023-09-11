﻿// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using System.Net;
using DiscordBot.BinaryUtils;

namespace DiscordBot.Network.Socket;

public class SocketData {

    public readonly IPAddress ipAddress;
    public readonly ushort port;
    public readonly IPEndPoint ipEndPoint;

    public readonly List<BinaryStream> outbound = new();
    public readonly List<BinaryStream> inbound = new();

    public SocketData(string ipAddress = "0.0.0.0", ushort port = 22222) {
        this.ipAddress = IPAddress.Parse(ipAddress);
        this.port = port;
        this.ipEndPoint = new(this.ipAddress, this.port);
    }

    public BinaryStream? ReadInbound() {
        if(this.inbound.Count == 0) {
            return null;
        }
        BinaryStream c = this.inbound.First();
        this.inbound.RemoveAt(0);
        return c;
    }

    public BinaryStream? ReadOutbound() {
        if(this.outbound.Count == 0) {
            return null;
        }
        BinaryStream c = this.outbound.First();
        this.outbound.RemoveAt(0);
        return c;
    }

    public void WriteInbound(BinaryStream data) {
        this.inbound.Add(data);
    }

    public void WriteOutbound(BinaryStream data) {
        this.outbound.Add(data);
    }
}