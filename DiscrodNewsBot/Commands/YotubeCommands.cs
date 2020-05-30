using System;
using System.Collections.Generic;
using System.Linq;
using DiscordNewsBot.Resources;
using System.Threading.Tasks;
using DiscordNewsBot.Clients;
using DiscordNewsBot.Config;
using DiscordNewsBot.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace DiscordNewsBot.Commands
{
    [Group("yt")]
    internal class YotubeCommands
    {
        [Command("search"), Description("Searches some video on Youtube")]
        public async Task Search(CommandContext ctx, [Description("Search query")]params string[] query)
        {
            await ctx.TriggerTypingAsync();
            var interactivity = ctx.Client.GetInteractivityModule();

            var searchResult = await YouTubeClient.GetInstance().Search(string.Join(" ", query), 25);
            var resultPages = new List<Page>();
            foreach (var searchChunk in searchResult.FastSplit(5))
            {
                var page = new Page
                {
                    Embed = new DiscordEmbedBuilder
                    {
                        Title = string.Format(
                            Resource.ResourceManager.GetString("page-text", ctx.Channel.GetCultureInfo()),
                            resultPages.Count + 1),
                        Description = string.Join("", searchChunk)
                    }
                };
                resultPages.Add(page);
            }
            
            await interactivity.SendPaginatedMessage(ctx.Channel, ctx.User, resultPages, TimeSpan.FromMinutes(5), TimeoutBehaviour.Delete);
        }

        [Command("add"), Description("Searches and add YouTube channel to notification list"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task AddChannel(CommandContext ctx, [Description("Channel name")]params string[] channelName)
        {
            await ctx.TriggerTypingAsync();

            var nameFull = string.Join(" ", channelName);
            string youTubeChannelId;
            string youTubeChannelName;

            try
            {
                var channelsList = await YouTubeClient.GetInstance().SearchChannel(nameFull);
                youTubeChannelId = channelsList.Count > 0 ? channelsList.First().Value : "";
                youTubeChannelName = channelsList.Count > 0 ? channelsList.First().Key : "";
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(Resources.Resource.ResourceManager.GetString("add-channel-exception-text", ctx.Channel.GetCultureInfo()));
                Program.Log(ex.Source, ex.Message, DSharpPlus.LogLevel.Error);
                return;
            }

            if (youTubeChannelId == string.Empty)
            {
                await ctx.RespondAsync(Resource.ResourceManager.GetString("add-channel-not-found-text", ctx.Channel.GetCultureInfo()));
                return;
            }
            
            if (SettingsManager.Config.ChannelList.ContainsChannel(ctx.Channel.Id, youTubeChannelId) != null)
            {
                await ctx.RespondAsync(Resource.ResourceManager.GetString("add-channel-already-exist-text", ctx.Channel.GetCultureInfo()));
                return;
            }

            SettingsManager.Config.ChannelList.Add(new ChatChannel(ctx.Channel.Id, youTubeChannelId, typeof(YouTubeClient), youTubeChannelName));

            await ctx.RespondAsync(Resource.ResourceManager.GetString("add-channel-success-text", ctx.Channel.GetCultureInfo()));
        }

        [Command("del"), Description("Remove YouTube channel from notifications"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task RemoveChannel(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var interactivity = ctx.Client.GetInteractivityModule();
        
            if (!SettingsManager.Config.ChannelList.ContainsKey(ctx.Channel.Id))
            {
                await ctx.RespondAsync(Resource.ResourceManager.GetString("channels-list-empty-text", ctx.Channel.GetCultureInfo()));
            }

            var resultPages = new List<Page>();
            var channelNames = new List<string>();
            var channelList = new List<int>();
            int numberOfChannels = 1;
            for (int i = 0; i < SettingsManager.Config.ChannelList.Count; i++)
            {
                if (SettingsManager.Config.ChannelList[i].ServiceType.Name.Equals("YouTubeClient")
                    && SettingsManager.Config.ChannelList[i].DiscordChatId == ctx.Channel.Id)
                {
                    channelNames.Add($"{EmojiesList.GetEmojiNumFromInt(numberOfChannels)} {SettingsManager.Config.ChannelList[i].ServiceChannelName} - https://www.youtube.com/channel/{SettingsManager.Config.ChannelList[i].ServiceChannel}{Environment.NewLine}");
                    channelList.Add(i);
                    numberOfChannels++;
                }
            }
            var message = await ctx.RespondAsync(embed: new DiscordEmbedBuilder
            {
                Description = $"{string.Join($"{Environment.NewLine}", channelNames)}\n{Resource.ResourceManager.GetString("youtube-delete-channel-select-text", ctx.Channel.GetCultureInfo())}"
            });

            int userSelection = -1;
            var userSelectionMessage =
                await interactivity.WaitForMessageAsync(m => int.TryParse(m.Content, out userSelection));
            if (userSelection != -1)
            {
                SettingsManager.Config.ChannelList.RemoveAt( channelList[userSelection - 1]);
            }

            await ctx.RespondAsync("Канал успешно удален.");
        }

        [Command("list"), Description("List all added YouTube channels")]
        public async Task ListChannels(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var interactivity = ctx.Client.GetInteractivityModule();

            if (!SettingsManager.Config.ChannelList.ContainsKey(ctx.Channel.Id))
            {
                await ctx.RespondAsync(Resource.ResourceManager.GetString("channels-list-empty-text", ctx.Channel.GetCultureInfo()));
            }

            var resultPages = new List<Page>();
            var channelsList = new List<string>();
            for (int i = 0, n = 1; i < SettingsManager.Config.ChannelList.Count; i++)
            {
                if (SettingsManager.Config.ChannelList[i].ServiceType.Name.Equals("YouTubeClient") 
                    && SettingsManager.Config.ChannelList[i].DiscordChatId == ctx.Channel.Id)
                {
                    channelsList.Add($"{n}. {SettingsManager.Config.ChannelList[i].ServiceChannelName} - https://www.youtube.com/channel/{SettingsManager.Config.ChannelList[i].ServiceChannel}{Environment.NewLine}");
                    n++;
                }
            }

            foreach (var channelsChunk in channelsList.FastSplit(3))
            {
                var page = new Page
                {
                    Embed = new DiscordEmbedBuilder
                    {
                        Title = string.Format(
                            Resource.ResourceManager.GetString("page-text", ctx.Channel.GetCultureInfo()),
                            EmojiesList.GetEmojiNumFromInt(resultPages.Count + 1)),
                        Description = string.Join($"{Environment.NewLine}", channelsChunk)
                    }
                };
                resultPages.Add(page);
            }
            await interactivity.SendPaginatedMessage(ctx.Channel, ctx.User, resultPages, TimeSpan.FromMinutes(5), TimeoutBehaviour.Delete);
        }
    }
}
