using System.Globalization;
using System.Threading.Tasks;
using DiscordNewsBot.Config;
using DiscordNewsBot.Resources;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordNewsBot.Commands
{
    [RequireUserPermissions(DSharpPlus.Permissions.Administrator), Hidden]
    internal class AdminCommands
    {
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
            await ctx.RespondAsync(
                $"Оповещения {(state ? ":regional_indicator_o: :regional_indicator_n: " : ":regional_indicator_o: :regional_indicator_f: :regional_indicator_f: ")}");
        }

        [Command("setlang"), Description("Set bot language for current text channel. Not affect help command output")]
        public async Task SetLanguage(CommandContext ctx, string languageCode)
        {
            await ctx.TriggerTypingAsync();

            var newCulture =  new CultureInfo(languageCode);

            if (SettingsManager.Config.ChatCultures.ContainsKey(ctx.Channel.Id))
                SettingsManager.Config.ChatCultures[ctx.Channel.Id] = newCulture;
            else
                SettingsManager.Config.ChatCultures.Add(ctx.Channel.Id, newCulture);
           
            await ctx.RespondAsync(Resource.ResourceManager.GetString("lang-selected-text", ctx.Channel.GetCultureInfo()));
        }

        [Command("save"), Description("Save cached bot settings")]
        public async Task Save(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            SettingsManager.Config.Save();
        }

        [Command("load"), Description("Load setting from config file")]
        public async Task Load(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            SettingsManager.Load();
        }

        //[Command("ping"), Description("Show current ping from bot to Discord servers")]
        //public async Task Ping(CommandContext ctx)
        //{
        //    await ctx.TriggerTypingAsync();
        //    await ctx.RespondAsync(
        //        $"Пинг: {Program.Discord.Ping} {(Program.Discord.Ping < 350 ? ":thumbsup:" : ":thumbsdown:")}");
        //}
    }
}
