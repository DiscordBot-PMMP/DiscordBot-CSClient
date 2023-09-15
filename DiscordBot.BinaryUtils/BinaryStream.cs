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

// Sure could use a load of method overloading but I prefer type in func name

public class BinaryStream {

    private readonly List<byte> buffer;
    private int offset;

    public bool Eof { get { return this.offset == this.buffer.Count; }}

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

    public void Put(byte singleByte) {
        this.buffer.Add(singleByte);
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

    public void PutStringArray(string[] v) {
        this.PutInt((uint)v.Length);
        foreach(string va in v) {
            this.PutString(va);
        }
    }

    public ulong[] GetLongArray() {
        uint size = this.GetInt();
        ulong[] longs = new ulong[size];
        for(int j = 0; j < size; j++) {
            longs[j] = this.GetLong();
        }
        return longs;
    }

    public void PutLongArray(ulong[] v) {
        this.PutInt((uint)v.Length);
        foreach(ulong va in v) {
            this.PutLong(va);
        }
    }

    public uint[] GetIntArray() {
        uint size = this.GetInt();
        uint[] ints = new uint[size];
        for(int j = 0; j < size; j++) {
            ints[j] = this.GetInt();
        }
        return ints;
    }

    public void PutIntArray(uint[] v) {
        this.PutInt((uint)v.Length);
        foreach(uint va in v) {
            this.PutInt(va);
        }
    }

    public ushort[] GetShortArray() {
        uint size = this.GetInt();
        ushort[] shorts = new ushort[size];
        for(int j = 0; j < size; j++) {
            shorts[j] = this.GetShort();
        }
        return shorts;
    }

    public void PutShortArray(ushort[] v) {
        this.PutInt((uint)v.Length);
        foreach(ushort va in v) {
            this.PutShort(va);
        }
    }

    public byte[] GetByteArray() {
        return this.Get((int)this.GetInt());
    }

    public void PutByteArray(byte[] v) {
        this.PutInt((uint)v.Length);
        this.Put(v);
    }

    public string? GetNullableString() {
        return this.GetBool() ? this.GetString() : null;
    }

    public void PutNullableString(string? v) {
        this.PutBool(v != null);
        if(v != null) {
            this.PutString(v);
        }
    }

    public ulong? GetNullableLong() {
        return this.GetBool() ? this.GetLong() : null;
    }

    public void PutNullableLong(ulong? v) {
        this.PutBool(v != null);
        if(v != null) {
            this.PutLong((ulong)v);
        }
    }

    public uint? GetNullableInt() {
        return this.GetBool() ? this.GetInt() : null;
    }

    public void PutNullableInt(uint? v) {
        this.PutBool(v != null);
        if(v != null) {
            this.PutInt((uint)v);
        }
    }

    public ushort? GetNullableShort() {
        return this.GetBool() ? this.GetShort() : null;
    }

    public void PutNullableShort(ushort? v) {
        this.PutBool(v != null);
        if(v != null) {
            this.PutShort((ushort)v);
        }
    }

    public byte? GetNullableByte() {
        return this.GetBool() ? this.GetByte() : null;
    }

    public void PutNullableByte(byte? v) {
        this.PutBool(v != null);
        if(v != null) {
            this.PutByte((byte)v);
        }
    }

    public bool? GetNullableBool() {
        return this.GetBool() ? this.GetBool() : null;
    }

    public void PutNullableBool(bool? v) {
        this.PutBool(v != null);
        if(v != null) {
            this.PutBool((bool)v);
        }
    }

    public string GetString() {
        return Encoding.UTF8.GetString(this.Get((int)this.GetInt()));    
    }

    public void PutString(string s) {
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        this.PutInt((uint)bytes.Length);
        this.Put(bytes);
    }

    public ulong GetLong() {
        return BinaryPrimitives.ReadUInt64BigEndian(this.Get(8));
    }

    public void PutLong(ulong v) {
        byte[] bytes = new byte[8];
        BinaryPrimitives.WriteUInt64BigEndian(new Span<byte>(bytes), v);
        this.Put(bytes);
    }

    public uint GetInt() {
        return BinaryPrimitives.ReadUInt32BigEndian(this.Get(4));
    }

    public void PutInt(uint v) {
        byte[] bytes = new byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(new Span<byte>(bytes), v);
        this.Put(bytes);
    }

    public ushort GetShort() {
        return BinaryPrimitives.ReadUInt16BigEndian(this.Get(2));
    }

    public void PutShort(ushort v) {
        byte[] bytes = new byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(new Span<byte>(bytes), v);
        this.Put(bytes);
    }

    public byte GetByte() {
        return this.Get(1)[0];
    }

    public void PutByte(byte v) {
        this.Put(v);
    }

    public bool GetBool() {
        return this.GetByte() == 1;
    }

    public void PutBool(bool v) {
        this.PutByte((byte)(v ? 1 : 0));
    }

    public override string ToString() {
        return "BinaryStream <" + Convert.ToHexString(this.buffer.ToArray()) + ">";
    }
}
