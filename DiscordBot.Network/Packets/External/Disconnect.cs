// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using DiscordBot.BinaryUtils;

namespace DiscordBot.Network.Packets.External;

sealed public class Disconnect : Packet, IInboundPacket, IOutboundPacket {

    public string Message { get; set; } = "Unknown";

    public new static ushort Id => 101;
    public Disconnect(bool? Uid = true) : base(Uid) { }

    public BinaryStream BinarySerialize() {
        BinaryStream bs = new();
        bs.PutInt(this.UID);
        bs.PutString(this.Message);
        return bs;
    }

    public void FromBinary(BinaryStream binaryStream) {
        this.UID = binaryStream.GetInt();
        this.Message = binaryStream.GetString();
    }

    public override void Handle() {
        PacketHandler<Disconnect>.Invoke(Disconnect.Id, this);
    }
}
