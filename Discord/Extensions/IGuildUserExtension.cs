using Discord;

namespace SaveHope.Felarel.Discord.Extensions;

public static class IGuildUserExtension
{
    public static IRole GetHighestRole(this IGuildUser user)
        => user.RoleIds.Select(id => user.Guild.GetRole(id)).MaxBy(r => r.Position)!;
}
