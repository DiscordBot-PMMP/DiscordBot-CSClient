// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using System.Net.Sockets;

namespace DiscordBot.Socket;

public class Socket {

    protected readonly SocketData socketData;
    protected System.Net.Sockets.Socket socket;

    private Client? client = null;

    public Socket(SocketData socketData) {
        this.socketData = socketData;
        this.socket = new(this.socketData.ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        this.socket.Bind(this.socketData.ipEndPoint);
    }

    public void Listen() {
        this.socket.Listen();
    }

    public async Task<Client> AcceptClient() {
        if(this.client != null) {
            throw new Exception("Client already connected, disconnect before re-accepting new client.");
        }
        this.client = new Client(await this.socket.AcceptAsync());
        return this.client;
    }

    public void DisconnectClient(bool close = true) {
        if(close) {
            this.client?.Close();
        }
        this.client = null;
    }
}
