// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using DiscordBot.BinaryUtils;

namespace DiscordBot.Network.Packets.External;

sealed public class Disconnect : Packet {

    public string Message { get; set; } = "Unknown";

    public new static ushort Id => 101;
    public Disconnect(bool? Uid = true) : base(Uid) { }

    public override BinaryStream BinarySerialize() {
        BinaryStream bs = new();
        bs.PutInt(this.UID);
        bs.PutString(this.Message);
        return bs;
    }

    public override void FromBinary(BinaryStream binaryStream) {
        base.FromBinary(binaryStream);
        this.Message = binaryStream.GetString();
    }

    public override void Handle() {
        PacketHandler<Disconnect>.Invoke(Disconnect.Id, this);
    }
}
