// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using System.Text;

namespace DiscordBot_CSClient;

public static class Binary {

    public static string GetString(byte[] buf, int i) {
        return Encoding.UTF8.GetString(buf, i + 4, GetInt(buf, i));
    }

    public static int GetLong(byte[] buf, int i) {
        return (buf[i] << 40) | (buf[i + 1] << 32) | (buf[i + 2] << 24) | (buf[i + 3] << 16) | (buf[i + 4] << 8) | buf[i + 5];
    }

    public static int GetInt(byte[] buf, int i) {
        return (buf[i] << 24) | (buf[i + 1] << 16) | (buf[i + 2] << 8) | buf[i + 3];
    }

    public static int GetShort(byte[] buf, int i) {
        return (buf[i] << 8) | buf[i + 1];
    }

    public static int GetByte(byte[] buf, int i) {
        return buf[i];
    }

    public static bool GetBool(byte[] buf, int i) {
        return GetByte(buf, i) == 1;
    }
}
