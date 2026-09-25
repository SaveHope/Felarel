using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SaveHope.Felarel.Discord.Models;

namespace SaveHope.Felarel.Discord.Services;

public class VoiceRoomService
{
    private ILogger _logger;

    private DiscordSocketClient _client;

    private VoiceRoomServiceConfig _config;

    private Dictionary<ulong, RestVoiceChannel> _activeRooms = [];

    public VoiceRoomService(ILogger<VoiceRoomService> logger, DiscordSocketClient client, IConfiguration config)
    {
        _logger = logger;
        _client = client;
        _config = config.GetSection("VoiceRoom").Get<VoiceRoomServiceConfig>() ?? new();
        _client.UserVoiceStateUpdated += _userVoiceStateUpdatedCallback;
        _client.Ready += _clientReadyCallback;

        _logger.LogInformation("[VoiceRoomsService] Создан");
    }

    private async Task _userVoiceStateUpdatedCallback(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        //  Создание голосовой комнаты
        if (_config.VoiceHubs.Contains(after.VoiceChannelId ?? 0))
        {
            var guild = after.VoiceChannel.Guild;
            var newvoice = await guild.CreateVoiceChannelAsync($"Комната {user.GlobalName}", 
                props => props.CategoryId = after.VoiceChannel.CategoryId);
            _activeRooms[newvoice.Id] = newvoice;

            await guild.GetUser(user.Id).ModifyAsync(state => state.ChannelId = newvoice.Id);
        }

        // Удаление пустой голосовой комнаты
        if (before.VoiceChannelId != after.VoiceChannelId 
            && _activeRooms.TryGetValue(before.VoiceChannelId ?? 0, out var voice))
        {
            if (before.VoiceChannel.ConnectedUsers.Count == 0)
            {
                await voice.DeleteAsync();
                _activeRooms.Remove(voice.Id);
            }
        }
    }

    private async Task _clientReadyCallback()
    {
        //  Проверка на наличие опустевших комнат за время дисконнекта бота
        foreach (var pair in _activeRooms)
        {
            if (await _client.GetChannelAsync(pair.Key) is SocketVoiceChannel voice && voice.ConnectedUsers.Count == 0)
                await voice.DeleteAsync();
        }
    }
}