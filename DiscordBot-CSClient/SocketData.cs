// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using System.Net;

namespace DiscordBot_CSClient;

public class SocketData{

    private readonly IPAddress ipAddress;
    private readonly ushort port;

    private List<List<byte>> outbound = new List<List<byte>>();
    private List<List<byte>> inbound = new List<List<byte>>();

    public SocketData(string ipAddress = "0.0.0.0", ushort port = 22222){
        this.ipAddress = IPAddress.Parse(ipAddress);
        this.port = port;
    }

    public IPAddress getIpAddress(){
        return ipAddress;
    }

    public ushort getPort(){
        return port;
    }
    
    public List<byte>? readInbound(){
        if(inbound.Count == 0) {
            return null;
        }
        var c = inbound.First();
        inbound.RemoveAt(0);
        return c;
    }

    public List<byte>? readOutbound() {
        if(outbound.Count == 0) {
            return null;
        }
        var c = outbound.First();
        outbound.RemoveAt(0);
        return c;
    }

    public void writeInbound(List<byte> data){
        inbound.Add(data);
    }

    public void writeOutbound(List<byte> data){
        outbound.Add(data);
    }
}
