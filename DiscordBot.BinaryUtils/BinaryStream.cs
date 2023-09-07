// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using System.Buffers.Binary;
using System.Text;

namespace DiscordBot.BinaryUtils;


public class BinaryStream {

    private readonly List<byte> buffer;
    private int offset;

    public BinaryStream(List<byte> buffer, int offset = 0) {
        this.offset = offset;
        this.buffer = buffer;
    }

    public BinaryStream(byte[]? buffer = null, int offset = 0) {
        this.offset = offset;
        this.buffer = buffer == null ? new() : buffer.ToList();
    }

    public byte[] GetBuffer() {
        return this.buffer.ToArray();
    }

    public byte[] Get(int size = 1) {
        if(this.offset + size > this.buffer.Count) {
            throw new EndOfStreamException("Attempted to read " + size + "bytes, only " + (this.buffer.Count - this.offset) + " remain.");
        }
        if(size == 0) {
            return Array.Empty<byte>();
        }
        this.offset += size;
        return this.buffer.GetRange(this.offset - size, size).ToArray();
    }

    public void Put(byte[] bytes) {
        this.buffer.AddRange(bytes);
    }

    public string[] GetStringArray() {
        uint size = this.GetInt();
        string[] strings = new string[size];
        for(int j = 0; j < size; j++) {
            strings[j] = this.GetString();
        }
        return strings;
    }

    public ulong[] GetLongArray() {
        uint size = this.GetInt();
        ulong[] longs = new ulong[size];
        for(int j = 0; j < size; j++) {
            longs[j] = this.GetLong();
        }
        return longs;
    }

    public uint[] GetIntArray() {
        uint size = this.GetInt();
        uint[] ints = new uint[size];
        for(int j = 0; j < size; j++) {
            ints[j] = this.GetInt();
        }
        return ints;
    }

    public ushort[] GetShortArray() {
        uint size = this.GetInt();
        ushort[] shorts = new ushort[size];
        for(int j = 0; j < size; j++) {
            shorts[j] = this.GetShort();
        }
        return shorts;
    }

    public byte[] GetByteArray() {
        return this.Get((int)this.GetInt());
    }

    public string? GetNullableString() {
        return this.GetBool() ? this.GetString() : null;
    }

    public ulong? GetNullableLong() {
        return this.GetBool() ? this.GetLong() : null;
    }

    public uint? GetNullableInt() {
        return this.GetBool() ? this.GetInt() : null;
    }

    public ushort? GetNullableShort() {
        return this.GetBool() ? this.GetShort() : null;
    }

    public byte? GetNullableByte() {
        return this.GetBool() ? this.GetByte() : null;
    }

    public bool? GetNullableBool() {
        return this.GetBool() ? this.GetBool() : null;
    }

    public string GetString() {
        return Encoding.UTF8.GetString(this.Get((int)this.GetInt()));    
    }

    public ulong GetLong() {
        return BinaryPrimitives.ReadUInt64BigEndian(this.Get(8));
    }

    public uint GetInt() {
        return BinaryPrimitives.ReadUInt32BigEndian(this.Get(4));

    }

    public ushort GetShort() {
        return BinaryPrimitives.ReadUInt16BigEndian(this.Get(2));
        
    }

    public byte GetByte() {
        return this.Get(1)[0];
    }

    public bool GetBool() {
        return this.GetByte() == 1;
    }
}

