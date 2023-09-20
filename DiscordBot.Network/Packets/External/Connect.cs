// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using DiscordBot.BinaryUtils;

namespace DiscordBot.Network.Packets.External;

sealed public class Connect : Packet {

    public byte Version { get; set; }
    public uint Magic { get; set; }

    public new static ushort Id => 100;
    public Connect(bool? Uid = true) : base(Uid) { }

    public override BinaryStream BinarySerialize() {
        BinaryStream bs = new();
        bs.PutInt(this.UID);
        bs.PutByte(this.Version);
        bs.PutInt(this.Magic);
        return bs;
    }

    public override void FromBinary(BinaryStream binaryStream) {
        base.FromBinary(binaryStream);
        this.Version = binaryStream.GetByte();
        this.Magic = binaryStream.GetInt();
    }

    public override void Handle() {
        PacketHandler<Connect>.Invoke(Connect.Id, this);
    }
}
