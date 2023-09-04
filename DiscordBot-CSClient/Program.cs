// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Starting socket.");

IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
IPEndPoint ipEndPoint = new(ipAddress, 22_222);

Socket listener = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

listener.Bind(ipEndPoint);
listener.Listen(100);

Console.WriteLine("Listening for new connections...");

var handler = await listener.AcceptAsync();

Console.WriteLine("Client connected, waiting for initial packet...");

while(true) {
    // Receive message.
    var raw_size = new byte[4];
    var received = await handler.ReceiveAsync(raw_size, SocketFlags.None);
    if(received != 4){
        throw new FormatException("4 bytes expected, received: " + received.ToString());
    }
    int size = getIntBE(raw_size, 0);
    Console.WriteLine();
    var data = new byte[size];
    received = await handler.ReceiveAsync(data, SocketFlags.None);
    if(received != size){
        throw new FormatException(size.ToString() + " bytes expected, received: " + received.ToString());
    }
    int packetId = getShortBE(data, 0);
    if(packetId != 100){
        throw new FormatException("Expected Connect packet (100), received: " + packetId.ToString());
    }
    int version = getByteBE(data, 6);
    int magic = getIntBE(data, 7);
    if(version != 2 || magic != 0x4A61786B){
        throw new InvalidDataException("Version/Magic does not match expected. (" + version.ToString() + ", " + magic.ToString("X2"));
    }
    Console.WriteLine("Recieved connect packet, version: " + version.ToString() + " , magic: 0x" + magic.ToString("X2"));
    //await handler.SendAsync(raw_size, SocketFlags.None);
    //await handler.SendAsync(data, SocketFlags.None); pings back exact same connect packet for testing.
}

#pragma warning disable CS8321 // Local function is declared but never used
static int getLongBE(byte[] buf, int i) {
    return (buf[i] << 40) | (buf[i + 1] << 32) | (buf[i + 2] << 24) | (buf[i + 3] << 16) | (buf[i + 4] << 8) | buf[i + 5];
}

static int getIntBE(byte[] buf, int i) {
    return (buf[i] << 24) | (buf[i + 1] << 16) | (buf[i + 2] << 8) | buf[i + 3];
}

static int getShortBE(byte[] buf, int i) {
    return (buf[i] << 8) | buf[i + 1];
}

static int getByteBE(byte[] buf, int i) {
    return buf[i];
}
#pragma warning restore CS8321 // Local function is declared but never used
