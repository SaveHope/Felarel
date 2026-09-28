using Discord;
using Discord.Interactions;

namespace SaveHope.Felarel.Discord.Modules;

public class UtilsModule : InteractionModuleBase
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
}
