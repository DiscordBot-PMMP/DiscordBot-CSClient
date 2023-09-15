// DiscordBot-CSClient
//
// Licensed under the Open Software License version 3.0 (OSL-3.0)
// Copyright (C) 2023-present JaxkDev
//
// Discord :: JaxkDev
// Email   :: JaxkDev@gmail.com

using DiscordBot;
using System.Dynamic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

Thread.CurrentThread.Name = "MainThread";

string version = Assembly.GetCallingAssembly().GetName().Version?.ToString() ?? throw new Exception("Cannot fetch version.");
Console.WriteLine("DiscordBot C# Client - v" + version);
Console.WriteLine("By JaxkDev (c) OSL-3.0\n");

IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
    .AddEnvironmentVariables()
    .Build();

Settings settings;
try {
    settings = config.GetRequiredSection("Settings").Get<Settings>() ?? throw new ApplicationException("No settings could be loaded.");
} catch(Exception) {
    Console.WriteLine("Failed to load settings.");
    return;
}

if(args.Length > 0) {
    //Handle CLI.
    switch(args[0].ToLower()) {
        case "set-token":
            if(args.Length >= 2) {
                string token = args[1];
                Console.WriteLine("Writing token to secrets file...");
                string? secretsId = Assembly.GetExecutingAssembly().GetCustomAttribute<UserSecretsIdAttribute>()?.UserSecretsId;
                if(secretsId == null) {
                    Console.WriteLine("Failed to retreive secrets, no secretID.");
                    return;
                }
                string secretsPath = PathHelper.GetSecretsPathFromSecretsId(secretsId);
                string secretsJson = File.ReadAllText(secretsPath);
                dynamic secrets = JsonConvert.DeserializeObject<ExpandoObject>(secretsJson, new ExpandoObjectConverter()) ?? new ExpandoObject();
                secrets.Settings.Discord.token = token;

                string updatedSecretsJson = JsonConvert.SerializeObject(secrets, Formatting.Indented);
                File.WriteAllText(secretsPath, updatedSecretsJson);
                Console.WriteLine("Token updated, run program normally to start.");
            } else {
                Console.WriteLine("Usage: ./" + Assembly.GetExecutingAssembly().GetName().Name + " set-token TOKEN-HERE");
            }
            break;
        case "about":
        case "ver":
        case "version":
        case "-ver":
        case "-version":
            Console.WriteLine("Version: {0}", version);
            Console.WriteLine("CLR Version {0}", Environment.Version.ToString());
            Console.WriteLine("Operating System: {0}", Environment.OSVersion.ToString());
            break;
        default:
            Console.WriteLine("Unknown CLI option '" + args[0] + "'.");
            break;
    }
    return;
}

if(settings.Discord.token.Length <= 20) {
    Console.WriteLine("You must set-up a discord token before starting the program,\nSee https://github.com/DiscordBot-PMMP/DiscordBot-CSClient/wiki to set up your bot and retreive a token.");
    Console.WriteLine("Then apply your token with:\n./" + Assembly.GetExecutingAssembly().GetName().Name + " set-token TOKEN-HERE\n\nPress enter to quit.");
    Console.ReadLine();
    return;
}

DiscordBot.Network.Socket.Socket socket = new(new DiscordBot.Network.Socket.SocketData());
Thread socketThread = new(socket.Start);
socketThread.Start();

// Build a config object, using env vars and JSON providers.

//Console.WriteLine(settings.Discord.token);

//Discord here.

Console.ReadLine();

socket.Stop();
