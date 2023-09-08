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
Console.WriteLine("Socket listening on " + socketData.ipAddress + ":" + socketData.port);

while(true) {

    Console.WriteLine("Waiting for connection...");

    Client client = await socket.AcceptClient();

    Console.WriteLine("Client connected, waiting for initial packet...");

    // Receive packet.
    BinaryStream stream = await client.ReadAsync();

    // --- Connect packet. ---

    ushort packetId = stream.GetShort();

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

    BinaryStream response = new();
    response.PutShort(100); //packet
    response.PutInt(0); //uid
    response.PutByte(2); //ver
    response.PutInt(0x4A61786B); //magic

    BinaryStream d = new();
    d.PutInt((uint)response.GetBuffer().Length);
    d.Put(response.GetBuffer());

    await client.WriteAsync(d);

    // Connected !

    while(true) {
        stream = await client.ReadAsync();

        packetId = stream.GetShort();
        uid = stream.GetInt();

        Console.WriteLine("Received packet " + packetId + " (" + uid + ") - " + stream.ToString());

        if(packetId == 1) {
            //lazy bounce back heartbeat for testing.
            d = new();
            d.PutInt((uint)stream.GetBuffer().Length);
            d.Put(stream.GetBuffer());
            _ = client.WriteAsync(d);
        }else if(packetId == 101) {
            //disconnect
            string message = stream.GetString();
            Console.WriteLine($"Client Disconnected, message: \"{message}\"");
            socket.DisconnectClient();
            break;
        }
    }

    // ---------
}
