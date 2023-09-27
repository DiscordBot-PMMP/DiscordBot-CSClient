// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using DiscordBot.BinaryUtils;

namespace DiscordBot.Network.Packets.Misc;

sealed public class Heartbeat : Packet, IInboundPacket, IOutboundPacket {

    public uint Timestamp { get; set; }

    public new static ushort Id => 1;
    public Heartbeat(bool? Uid = true) : base(Uid) { }

    public BinaryStream BinarySerialize() {
        BinaryStream bs = new();
        bs.PutInt(this.UID);
        bs.PutInt(this.Timestamp);
        return bs;
    }

    public void FromBinary(BinaryStream binaryStream) {
        this.UID = binaryStream.GetInt();
        this.Timestamp = binaryStream.GetInt();
    }

    public override void Handle() {
        PacketHandler<Heartbeat>.Invoke(Heartbeat.Id, this);
    }
}
