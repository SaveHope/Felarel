using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SaveHope.Felarel.Discord.Models;
using Timer = System.Timers.Timer;

namespace SaveHope.Felarel.Discord.Services;

public class SplashService
{
    private ILogger _logger;

    private DiscordSocketClient _client;

    private SplashServiceConfig _config;

    private List<string> _splashs;

    private Timer _timer;

    private Random _random;

    public SplashService(ILogger<SplashService> logger, DiscordSocketClient client, IConfiguration config)
    {
        _logger = logger;
        _client = client;
        _config = config.GetSection("VoiceRoom").Get<SplashServiceConfig>() ?? new();
        _splashs = Parse(File.ReadAllLines("Config/splashs.txt"));

        _timer = new Timer(_config.Interval)
        {
            AutoReset = true
        };
        _timer.Elapsed += async (sender, args) => UpdateStatus();
        _timer.Start();
        _random = new Random();

        _logger.LogInformation("[SplashService] Создан");
    }

    private async Task UpdateStatus()
    {
        if (_client.LoginState == LoginState.LoggedIn)
            await _client.SetCustomStatusAsync(_splashs[_random.Next(_splashs.Count)]);
    }

    private List<string> Parse(string[] lines)
    {
        var list = new List<string>();

        foreach (var line in lines)
        {
            var templine = line;
            var pos = templine.IndexOf("//");
            if (pos != -1) templine = line[..pos];

            templine = templine.Trim();
            if (templine.Length == 0) continue;

            list.Add(templine);
        }

        return list;
    }
}
