using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using SaveHope.Felarel.Discord.Services;

namespace SaveHope.Felarel.Discord.Modules;

public class UtilsModule(DiscordSocketClient client, MainService mainService) : InteractionModuleBase
{
    [SlashCommand("avatar", "Shows user's avatar")]
    [CommandContextType(InteractionContextType.Guild)]
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
        int members = client.Guilds.Sum(g => g.MemberCount);
        int channels = client.Guilds.Sum(g => g.Channels.Count);
        TimeSpan uptime = DateTime.Now - mainService.ReadyAt;

        await RespondAsync(embed: new EmbedBuilder()
        {
            Title = "Bot status",
            Color = new(0xA561E1),
            Fields = [
                new() { Name = "Ping", Value = $"```yaml\n{client.Latency}ms\n```", IsInline = true },
                new() { Name = "Uptime", Value = $"```yaml\n{uptime:dd'd 'hh\\:mm\\:ss}\n```", IsInline = true },
                new() { Name = "Servers", Value = $"```yaml\n{client.Guilds.Count}\n```", IsInline = true },
                new() { Name = "Channels", Value = $"```yaml\n{channels}\n```", IsInline = true },
                new() { Name = "Members", Value = $"```yaml\n{members}\n```", IsInline = true }
            ]
        }.Build());
    }
}
