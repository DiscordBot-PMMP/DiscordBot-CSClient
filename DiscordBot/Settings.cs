// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

namespace DiscordBot;

public sealed class Settings {
    public DiscordSettings Discord { get; set; } = null!;
    public NetworkSettings Network { get; set; } = null!;
}

public sealed class DiscordSettings {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Not needed")]
    public string token { get; set; } = null!;
}

public sealed class NetworkSettings {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Not needed")]
    public string ip { get; set; } = null!;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Not needed")]
    public int port { get; set; }

}