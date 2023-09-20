// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using DiscordBot.Network.Packets;

namespace DiscordBot.Network;

public static class PacketHandler<T> where T : Packet {

    private static readonly object _locker = new();
    private static readonly Dictionary<ushort, List<Action<T>>> handlers = new();

    public static void AddHandler(ushort pid, Action<T> action) {
        lock(PacketHandler<T>._locker) {
            List<Action<T>>? lis = PacketHandler<T>.handlers.GetValueOrDefault(pid);
            if(lis != null) {
                lis.Add(action);
            } else {
                lis = new() {
                    action
                };
                PacketHandler<T>.handlers.Add(pid, lis);
            }
        }
    }

    public static void RemoveHandler(ushort pid, Action<T> action) {
        lock(PacketHandler<T>._locker) {
            List<Action<T>> lis = PacketHandler<T>.handlers.GetValueOrDefault(pid) ?? new List<Action<T>>();
            lis.Remove(action);
            PacketHandler<T>.handlers.Remove(pid);
            PacketHandler<T>.handlers.Add(pid, lis);
        }
    }

    public static int Invoke(ushort pid, T type) {
        lock(PacketHandler<T>._locker) {
            List<Action<T>> lis = PacketHandler<T>.handlers.GetValueOrDefault(pid) ?? new List<Action<T>>();
            foreach(Action<T> action in lis) {
                action.Invoke(type);
            }
            return lis.Count;
        }
    }
}
