using System;
using System.Diagnostics;
using System.Reflection;
using DiscordNewsBot.Resources;
using System.Threading.Tasks;
using DiscordNewsBot.Config;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DiscordNewsBot.Commands
{
    internal class MainCommands
    {
        [Command("joinserver"), Description("Create bot invite link")]
        public async Task JoinServer(CommandContext ctx, DiscordMember member = null)
        {
            await ctx.TriggerTypingAsync();
            var inviteLink =
                $"{Resource.ResourceManager.GetString("joinserver-link-text", ctx.Channel.GetCultureInfo())}: https://discordapp.com/oauth2/authorize?client_id={SettingsManager.Config.ClientId}&scope=bot&permissions={SettingsManager.Config.BotPermissions}";

            if (member == null)
            {
                await ctx.RespondAsync(inviteLink);
            }
            else
            {
                await ctx.RespondAsync($"{Resource.ResourceManager.GetString("joinserver-link-to-user", ctx.Channel.GetCultureInfo())} {member.DisplayName}");
                await member.SendMessageAsync(inviteLink);
            }
        }

        [Command("stats"), Description("Show bot statistics")]
        public async Task Status(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            var uptimeDelta = System.DateTime.Now - Program.StartTime;
            var embedFooter = new DiscordEmbedBuilder.EmbedFooter();
            embedFooter.Text = "dNewsBot by geckon01";
            
            await ctx.RespondAsync(embed: new DiscordEmbedBuilder
            {
                Title = "Stats",
                Description = string.Format(Resource.ResourceManager.GetString("statistics-text", ctx.Channel.GetCultureInfo()),
                    Program.Discord.Ping, Program.Discord.Guilds.Count, Program.Discord.CurrentUser.Username, $"{uptimeDelta.Days} days {uptimeDelta.Hours}h {uptimeDelta.Minutes:n0}m {uptimeDelta.Seconds:n0}s", (Process.GetCurrentProcess().WorkingSet64 / 1024) / 1024, version),
                Footer = embedFooter,
                Color = DiscordColor.Blurple
            });
        }
    }
}
