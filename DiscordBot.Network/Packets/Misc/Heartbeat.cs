// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using DiscordBot.BinaryUtils;

namespace DiscordBot.Network.Packets.Misc;

sealed public class Heartbeat : Packet {

    public uint Timestamp { get; set; }

    public new static ushort Id => 1;
    public Heartbeat(bool? Uid = true) : base(Uid) { }

    public override BinaryStream BinarySerialize() {
        BinaryStream bs = new();
        bs.PutInt(this.UID);
        bs.PutInt(this.Timestamp);
        return bs;
    }

    public override void FromBinary(BinaryStream binaryStream) {
        base.FromBinary(binaryStream);
        this.Timestamp = binaryStream.GetInt();
    }
}
