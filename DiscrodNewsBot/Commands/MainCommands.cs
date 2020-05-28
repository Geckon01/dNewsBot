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

    [Group("limit")]
    internal class LimitingCommands
    {
        [Command("clean"), Description("Clean text chat channel from bot messages and user commands")]
        public async Task Clean(CommandContext ctx, [Description("Maximum messages to delete(at least 100 max)")]int limit)
        {
            await ctx.TriggerTypingAsync();

            var messages = await ctx.Channel.GetMessagesAsync(limit);
            int deletedCount = 0;
            foreach (var message in messages)
            {
                if (message.Author.Username == Program.Discord.CurrentUser.Username || message.Content.StartsWith(SettingsManager.Config.CommandPrefix))
                {
                    try
                    {
                        await ctx.Channel.DeleteMessageAsync(message);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        Program.Log(typeof(LimitingCommands).Name, ex.Message, DSharpPlus.LogLevel.Error);
                    }
                    finally
                    {
                        await Task.Delay(150);
                    }
                }
            }

            var infoMessage = await ctx.RespondAsync(
                embed: new DiscordEmbedBuilder
            {
                Title = "Cleanup!",
                Description = string.Format(Resource.ResourceManager.GetString("deleted-messages-text", ctx.Channel.GetCultureInfo()), deletedCount)
            });
            await Task.Delay(5000);
            await infoMessage.DeleteAsync();
        }
    }
}
