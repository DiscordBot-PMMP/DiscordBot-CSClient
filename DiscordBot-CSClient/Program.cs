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

using Socket listener = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

listener.Bind(ipEndPoint);
listener.Listen(100);

Console.WriteLine("Listening for new connections...");

var handler = await listener.AcceptAsync();

Console.WriteLine("Client connected, waiting for initial packet...");

while(true) {
    // Receive message.
    var buffer = new byte[1_024];
    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);

    Console.WriteLine(response);
}
