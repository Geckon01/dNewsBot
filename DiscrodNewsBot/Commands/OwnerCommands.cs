using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DiscordNewsBot.Config;
using DiscordNewsBot.Resources;

namespace DiscordNewsBot.Commands
{
    [Group("sudo")]
    [RequireOwner, Hidden]
    internal class OwnerCommands
    {
        [Command("force"), Description("Force to check for new events")]
        public async Task ForceCheck(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            Program.ForceCheck();
            await ctx.RespondAsync(Resource.ResourceManager.GetString("force-check-text", ctx.Channel.GetCultureInfo()));
        }

        [Command("shutdown"), Description("Safely shutting down the bot with saving all settings")]
        public async Task Shutdown(CommandContext ctx, [Description("Shutdown timer in minutes. Skip this to shutdown immediately")] int minutes = 0)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync(string.Format(Resource.ResourceManager.GetString("shutdown-time-text", ctx.Channel.GetCultureInfo()), minutes));

            System.Threading.Thread.Sleep(minutes * 60000);

            await ctx.RespondAsync("👋");
            await Program.Shutdown();
        }

        [Command("mute"), Description("Enable/disable notifications on current text channel")]
        public async Task Mute(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            if (SettingsManager.Config.MutedServers.Contains(ctx.Channel.Id))
            {
                SettingsManager.Config.MutedServers.Remove(ctx.Channel.Id);
            }
            else
            {
                SettingsManager.Config.MutedServers.Add(ctx.Channel.Id);
            }

            var state = !SettingsManager.Config.MutedServers.Contains(ctx.Channel.Id);
            await ctx.RespondAsync(string.Format(string.Format(Resource.ResourceManager.GetString("notifications-status-text", ctx.Channel.GetCultureInfo()), state ? ":regional_indicator_o: :regional_indicator_n: " : ":regional_indicator_o: :regional_indicator_f: :regional_indicator_f: ")));
        }

        [Command("uptime"), Description("Show uptime")]
        public async Task Uptime(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var delta = System.DateTime.Now - Program.StartTime;
            await ctx.RespondAsync(string.Format(Resource.ResourceManager.GetString("bot-uptime-text", ctx.Channel.GetCultureInfo()), $"{delta.Days} days {delta.Hours}h {delta.Minutes:n0}m {delta.Seconds:n0}s"));
        }
    }
}
