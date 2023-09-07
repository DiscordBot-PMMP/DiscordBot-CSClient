// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com
using System;
namespace DiscordBot_CSClient;

public static class Binary {

    public static int GetLongBE(byte[] buf, int i) {
        return (buf[i] << 40) | (buf[i + 1] << 32) | (buf[i + 2] << 24) | (buf[i + 3] << 16) | (buf[i + 4] << 8) | buf[i + 5];
    }

    public static int GetIntBE(byte[] buf, int i) {
        return (buf[i] << 24) | (buf[i + 1] << 16) | (buf[i + 2] << 8) | buf[i + 3];
    }

    public static int GetShortBE(byte[] buf, int i) {
        return (buf[i] << 8) | buf[i + 1];
    }

    public static int GetByteBE(byte[] buf, int i) {
        return buf[i];
    }
}
