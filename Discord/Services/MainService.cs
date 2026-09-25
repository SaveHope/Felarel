using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SaveHope.Felarel.Discord.Services;

public class MainService
{
    public DateTime StartedAt { get; private set; } = new(0);

    public DateTime ReadyAt { get; private set; } = new(0);

    private ILogger _logger;

    private DiscordSocketClient _client;

    public MainService(ILogger<MainService> logger, DiscordSocketClient client)
    {
        _logger = logger;
        _client = client;
        _client.Log += _clientLogCallback;
        _client.Ready += _clientReadyCallback;
        _logger.LogInformation("[MainService] Создан");
    }

    public async void Launch(string token)
    {
        StartedAt = DateTime.Now;
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
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
    
    private async Task _clientLogCallback(LogMessage message)
        => _logger.Log(ToLogLevel(message.Severity), message.Exception, "[DiscordSocketClient] " + message.Message);
    
    private async Task _clientReadyCallback() => ReadyAt = DateTime.Now;
}
