// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using DiscordBot.Socket;

Thread.CurrentThread.Name = "MainThread";
Console.WriteLine("DiscordBot C# Client - v0.1.0");
Console.WriteLine("By JaxkDev (c) OSL-3.0\n");

Socket socket = new(new SocketData());
Thread socketThread = new(socket.Start);
socketThread.Start();


//Discord here.

Console.ReadLine();

socket.Stop();


