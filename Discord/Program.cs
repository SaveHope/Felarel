// See https://aka.ms/new-console-template for more information

using Discord.Net.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaveHope.Felarel.Discord.Models;
using SaveHope.Felarel.Discord.Services;

namespace SaveHope.Felarel.Discord;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Hewwo!!~~");

        string env = (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "dev").Trim().ToLowerInvariant();
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        Console.WriteLine($"Running in '{env}' environment");
        
        var config = BuildConfig(args, path, env);
        var provider = BuildProvider(config);
        InitServices(provider);
        Launch(provider, Path.Combine(path, "discord.token"));

        await Task.Delay(-1);
    }

    private static IConfigurationRoot BuildConfig(string[] args, string path, string env)
    {    
        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddYamlFile("appsettings.yaml", optional: true)
            .AddYamlFile($"appsettings.{env}.yaml", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        return builder.Build();
    }

    private static IServiceProvider BuildProvider(IConfiguration config)
    {
        var collection = new ServiceCollection()
            .AddLogging(builder =>
            {
                builder
                    .AddConfiguration(config.GetSection("Logging"))
                    .AddConsole()
                    .AddFile(config.GetSection("Logging:File"));
            })
            .AddSingleton(config)
            .AddSingleton(config.GetSection("Client").Get<DiscordSocketConfig>() ?? new())
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<RestClientProvider>()
            .AddSingleton(config.GetSection("Interaction").Get<AdvancedInteractionServiceConfig>() ?? new())
            .AddSingleton<AdvancedInteractionService>()

            .AddSingleton<MainService>();

        return collection.BuildServiceProvider();
    }

    private static void InitServices(IServiceProvider provider)
    {

    }

    private static void Launch(IServiceProvider provider, string tokenfile)
    {
        provider.GetRequiredService<MainService>()
            .Launch(File.ReadAllLines(tokenfile)[0]);
    }
}