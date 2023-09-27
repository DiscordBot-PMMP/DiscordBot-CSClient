// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using DiscordBot.BinaryUtils;
using DiscordBot.Network.Packets;
using DiscordBot.Network.Packets.External;
using DiscordBot.Network.Packets.Misc;

namespace DiscordBot.Network;

public sealed class NetworkAPI {

    public static readonly ushort VERSION = 2;
    public static readonly uint MAGIC = 0x4A61786B;

    public static readonly IReadOnlyDictionary<ushort, Func<BinaryStream, Packet>> PACKET_MAP;

    static NetworkAPI() {
        //Only IInboundPackets need registering here.
        NetworkAPI.PACKET_MAP = new Dictionary<ushort, Func<BinaryStream, Packet>>{
            /* 01-99 Misc Packets */
            /* 01 */ { Heartbeat.Id, new Func<BinaryStream, Heartbeat>((BinaryStream bs) => { Heartbeat h = new(false); h.FromBinary(bs); return h; }) },
            //{ 2, typeof(Resolution) },
            /* 03-99 Reserved */

            /* 100-199 External->PMMP Packets */
            /* 100 */ { Connect.Id, new Func<BinaryStream, Connect>((BinaryStream bs) => {Connect c = new(false); c.FromBinary(bs); return c; }) },
            /* 101 */ { Disconnect.Id, new Func<BinaryStream, Disconnect>((BinaryStream bs) => {Disconnect d = new(false); d.FromBinary(bs); return d; }) },
            /* 102-199 Reserved */
        };
    }

    public static ushort GetPacketId(Packet packet) {
        Type type = packet.GetType();
        return (ushort)(type.GetProperty("Id")?.GetValue(null) ?? throw new Exception("Failed to get ID"));
    }

    public static Packet GetPacket(BinaryStream binaryStream) {
        ushort pid = binaryStream.GetShort();
        Packet pk = NetworkAPI.PACKET_MAP[pid]?.Invoke(binaryStream) ?? throw new ArgumentOutOfRangeException($"PID '{pid}' does not exist.");
        if(!binaryStream.Eof) {
            Console.WriteLine("Warning: Unread bytes left in packet (" + pid + ", " + binaryStream.GetBuffer() + ")"); ;
        }
        return pk;
    }
}
