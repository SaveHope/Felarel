using Discord;
using Discord.Interactions;
using SaveHope.Felarel.Discord.Extensions;

namespace SaveHope.Felarel.Discord.Modules;

[Group("role", "Role commands")]
[CommandContextType(InteractionContextType.Guild)]
[DefaultMemberPermissions(GuildPermission.ManageRoles)]
public class RoleModule : InteractionModuleBase
{
    [SlashCommand("add", "Adds a role to the specified member")]
    public async Task AddRoleCommand([Summary(description: "Enter member's name")] IGuildUser user,
                                     [Summary(description: "Enter role's name")] IRole role)
    {
        await DeferAsync(ephemeral: true);
        string? errorMsg = null;

        if (!(Context.User as IGuildUser)!.GuildPermissions.ManageRoles)
            errorMsg = "You do not have enough rights to use this command!";
        else if (user.RoleIds.Contains(role.Id))
            errorMsg = $"User {user.DisplayName} already has {role.Mention} role!";
        else if ((Context.User as IGuildUser)!.GetHighestRole().Position <= role.Position)
            errorMsg = $"You do not have enough rights to give {role.Mention} role!";
        else if ((await Context.Guild.GetCurrentUserAsync()).GetHighestRole().Position <= role.Position)
            errorMsg = $"The bot doesn't have permission to give {role.Mention} role!";
        else
            await user.AddRoleAsync(role, new() { AuditLogReason = "Called manually by " + Context.User.Username }); // TODO(Ovl): Known bug - Application roles

        if (errorMsg is null)
        {
            await ModifyOriginalResponseAsync(mp => mp.Embed = new EmbedBuilder()
            {
                Title = "Success",
                Description = $"Added role {role.Mention} to user {user.DisplayName}!",
                Color = Color.Green
            }.Build());
        }
        else
        {
            await ModifyOriginalResponseAsync(mp => mp.Embed = new EmbedBuilder()
            {
                Title = "Fail",
                Description = errorMsg,
                Color = Color.Red
            }.Build());
        }
    }

    [SlashCommand("remove", "Removes a role from the specified member")]
    public async Task RemoveRoleCommand([Summary(description: "Enter member's name")] IGuildUser user,
                                        [Summary(description: "Enter role's name")] IRole role)
    {
        await DeferAsync(ephemeral: true);
        string? errorMsg = null;

        if (!(Context.User as IGuildUser)!.GuildPermissions.ManageRoles)
            errorMsg = "You do not have enough rights to use this command!";
        else if (Context.Guild.GetRole(role.Id).Position == 0)
            errorMsg = $"You can't remove the @everyone role!";
        else if (!user.RoleIds.Contains(role.Id))
            errorMsg = $"User {user.DisplayName} doesn't have {role.Mention} role!";
        else if ((Context.User as IGuildUser)!.GetHighestRole().Position <= role.Position)
            errorMsg = $"You do not have enough rights to remove {role.Mention} role!";
        else if ((await Context.Guild.GetCurrentUserAsync()).GetHighestRole().Position <= role.Position)
            errorMsg = $"The bot doesn't have permission to remove {role.Mention} role!";
        else
            await user.RemoveRoleAsync(role, new() { AuditLogReason = "Called manually by " + Context.User.Username }); // TODO(Ovl): Known bug - Application roles

        if (errorMsg is null)
        {
            await ModifyOriginalResponseAsync(msg => msg.Embed = new EmbedBuilder()
            {
                Title = "Success",
                Description = $"Removed role {role.Mention} from user {user.DisplayName}!",
                Color = Color.Green
            }.Build());
        }
        else
        {
            await ModifyOriginalResponseAsync(msg => msg.Embed = new EmbedBuilder()
            {
                Title = "Fail",
                Description = errorMsg,
                Color = Color.Red
            }.Build());
        }
    }

    [SlashCommand("list", "Shows all roles on the server")]
    public async Task ListRolesCommand()
    {
        await DeferAsync(ephemeral: true);

        var users = await Context.Guild.GetUsersAsync();
        var rolesListStr = string.Join('\n', Context.Guild.Roles.OrderByDescending(r => r.Position)
                                                                .Select(r => $"{r.Mention} ({users.Count(u => u.RoleIds.Contains(r.Id))} members)"));

        await ModifyOriginalResponseAsync(msg => msg.Embed = new EmbedBuilder()
        {
            Title = "Roles",
            Description = rolesListStr,
            Color = new(0xA561E1)
        }.Build());
    }
}
