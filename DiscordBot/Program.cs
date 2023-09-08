// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using DiscordBot.Socket;

Thread.CurrentThread.Name = "MainThread";

Console.WriteLine("Starting socket.");

SocketData socketData = new();
Socket socket = new(socketData);

Thread socketThread = new(() => {
    Thread.CurrentThread.Name = "SocketThread";
    socket.Start();
});

socketThread.Start();


//Discord here.

Console.ReadLine();

socket.Stop();


