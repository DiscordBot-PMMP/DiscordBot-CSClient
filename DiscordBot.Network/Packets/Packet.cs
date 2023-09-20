// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using DiscordBot.BinaryUtils;

namespace DiscordBot.Network.Packets;

public abstract class Packet : IBinarySerializable {

    public static ushort Id { get; }

    private static uint UID_COUNT = 1;
    public uint UID { get; private set; }

    public Packet(bool? Uid = true) {
        if(Uid ?? true) {
            if(UID_COUNT > 4294967295) {
                //32bit int overflow, reset.
                UID_COUNT = 1;
            }
            this.UID = UID_COUNT++;
        }
    }

    public abstract void Handle();

    //--- Binary ---//

    public virtual void FromBinary(BinaryStream binaryStream) {
        this.UID = binaryStream.GetInt();
    }

    public abstract BinaryStream BinarySerialize();
}
