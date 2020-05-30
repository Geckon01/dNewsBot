using DiscordNewsBot.Config;
using DiscordNewsBot.Resources;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordNewsBot.Commands
{
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
