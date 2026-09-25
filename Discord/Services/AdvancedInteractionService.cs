using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using SaveHope.Felarel.Discord.Models;
using Microsoft.Extensions.Logging;

namespace SaveHope.Felarel.Discord.Services;

public class AdvancedInteractionService : InteractionService
{
    private IServiceProvider _provider;

    private AdvancedInteractionServiceConfig _config;

    private ILogger _logger;

    private DiscordSocketClient _client;

    public AdvancedInteractionService(IServiceProvider provider, AdvancedInteractionServiceConfig config,
        ILogger<AdvancedInteractionService> logger, DiscordSocketClient client)
        : base(client.Rest, config)
    {
        _provider = provider;
        _config = config;
        _logger = logger;
        _client = client;
        _client.Ready += _clientReadyCallback;
        _client.InteractionCreated += _clientInteractionCreatedCallback;

        Log += _logCallback;

        _logger.LogInformation("[AdvancedInteractionService] Создан");
    }

        public LogLevel ToLogLevel(LogSeverity severity)
    {
        return severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.None,
        };
    }

    private async Task _clientReadyCallback()
    {
        await AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

        if (_config.DebugGuildId == 0)
        {
            await RegisterCommandsGloballyAsync();
        } 
        else
        {
            await RegisterCommandsToGuildAsync(_config.DebugGuildId);
        }
    }

    private async Task _clientInteractionCreatedCallback(SocketInteraction interaction)
        => await ExecuteCommandAsync(new SocketInteractionContext(_client, interaction), _provider);

    private async Task _logCallback(LogMessage message)
        => _logger.Log(ToLogLevel(message.Severity), message.Exception, message.Message, "[AdvancedInteractionService] " + message.Message);
}
