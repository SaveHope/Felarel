using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using SaveHope.Felarel.Discord.Services;

namespace SaveHope.Felarel.Discord.Modules;

public class UtilsModule(DiscordSocketClient client, MainService mainService) : InteractionModuleBase
{
    [SlashCommand("avatar", "Shows user's avatar")]
    public async Task AvatarCommand(IUser user)
    {
        IGuildUser guildUser = (IGuildUser)user;

        await RespondAsync(embed: new EmbedBuilder()
        {
            Title = $"{guildUser.DisplayName} avatar",
            ImageUrl = guildUser.GetAvatarUrl(),
            Color = new(0xA561E1)
        }.Build());
    }

    [SlashCommand("status", "Bot status panel")]
    public async Task StatusCommand()
    {
        int members = client.Guilds.Sum(g => { return g.MemberCount; });
        int channels = client.Guilds.Sum(g => { return g.Channels.Count; });
        TimeSpan uptime = DateTime.Now - mainService.ReadyAt;
        string uptime_str;
        if (uptime.Days > 0)
            uptime_str = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        else if (uptime.Hours > 0)
            uptime_str = $"{uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        else if (uptime.Minutes > 0)
            uptime_str = $"{uptime.Minutes}m {uptime.Seconds}s";
        else
            uptime_str = $"{uptime.Seconds}s";

        await RespondAsync(embed: new EmbedBuilder()
        {
            Title = "Bot status",
            Color = new(0xA561E1),
            Fields = [
                new() { Name = "Ping", Value = $"```yaml\n{client.Latency}ms\n```", IsInline = true },
                new() { Name = "Uptime", Value = $"```yaml\n{uptime_str}\n```", IsInline = true },
                new() { Name = "Servers", Value = $"```yaml\n{client.Guilds.Count}\n```", IsInline = true },
                new() { Name = "Channels", Value = $"```yaml\n{channels}\n```", IsInline = true },
                new() { Name = "Members", Value = $"```yaml\n{members}\n```", IsInline = true }
            ]
        }.Build());
    }
}
