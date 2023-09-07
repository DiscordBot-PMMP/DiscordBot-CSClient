// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using DiscordBot.BinaryUtils;
using DiscordBot.Socket;

Console.WriteLine("Starting socket.");

SocketData socketData = new();
Socket socket = new(socketData);

socket.Listen();

Console.WriteLine("Listening for new connections...");

var handler = await socket.Accept();

Console.WriteLine("Client connected, waiting for initial packet...");

while(true) {
    // Receive message.
    var raw_size = new byte[4];
    var received = await handler.ReceiveAsync(raw_size, System.Net.Sockets.SocketFlags.None);
    if(received != 4) {
        throw new FormatException("4 bytes expected, received: " + received.ToString());
    }
    BinaryStream init = new(raw_size);
    uint size = init.GetInt();
    var data = new byte[size];
    received = await handler.ReceiveAsync(data, System.Net.Sockets.SocketFlags.None);
    if(received != size) {
        throw new FormatException(size.ToString() + " bytes expected, received: " + received.ToString());
    }
    BinaryStream stream = new(data);
    ushort packetId = stream.GetShort();


    // --- Connect packet. ---

    if(packetId != 100) {
        throw new FormatException("Expected Connect packet (100), received: " + packetId.ToString());
    }
    uint uid = stream.GetInt();
    byte version = stream.GetByte();
    uint magic = stream.GetInt();
    if(version != 2 || magic != 0x4A61786B) {
        throw new InvalidDataException("Version/Magic does not match expected. (" + version.ToString() + ", " + magic.ToString("X2"));
    }
    Console.WriteLine($"Recieved connect packet ({uid}), version: {version} , magic: 0x"+ magic.ToString("X2"));

    // ---------

    //await handler.SendAsync(raw_size, SocketFlags.None);
    //await handler.SendAsync(data, SocketFlags.None); pings back exact same connect packet for testing.
}
