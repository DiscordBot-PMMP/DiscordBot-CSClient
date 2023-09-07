// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using System.Net;
using System.Net.Sockets;
using DiscordBot_CSClient;
using DiscordBot_CSClient.Socket;

Console.WriteLine("Starting socket.");

/*SocketData socketData = new();
DiscordBot_CSClient.Socket.Socket socket = new(socketData);*/

IPEndPoint endpoint = new(IPAddress.Parse("0.0.0.0"), 22222);

System.Net.Sockets.Socket listener = new(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

listener.Bind(endpoint);
listener.Listen(100);

Console.WriteLine("Listening for new connections...");

var handler = await listener.AcceptAsync();

Console.WriteLine("Client connected, waiting for initial packet...");

while(true) {
    // Receive message.
    var raw_size = new byte[4];
    var received = await handler.ReceiveAsync(raw_size, SocketFlags.None);
    if(received != 4) {
        throw new FormatException("4 bytes expected, received: " + received.ToString());
    }
    int size = Binary.GetIntBE(raw_size, 0);
    Console.WriteLine();
    var data = new byte[size];
    received = await handler.ReceiveAsync(data, SocketFlags.None);
    if(received != size) {
        throw new FormatException(size.ToString() + " bytes expected, received: " + received.ToString());
    }
    int packetId = Binary.GetShortBE(data, 0);
    if(packetId != 100) {
        throw new FormatException("Expected Connect packet (100), received: " + packetId.ToString());
    }
    int version = Binary.GetByteBE(data, 6);
    int magic = Binary.GetIntBE(data, 7);
    if(version != 2 || magic != 0x4A61786B) {
        throw new InvalidDataException("Version/Magic does not match expected. (" + version.ToString() + ", " + magic.ToString("X2"));
    }
    Console.WriteLine("Recieved connect packet, version: " + version.ToString() + " , magic: 0x" + magic.ToString("X2"));
    //await handler.SendAsync(raw_size, SocketFlags.None);
    //await handler.SendAsync(data, SocketFlags.None); pings back exact same connect packet for testing.
}
