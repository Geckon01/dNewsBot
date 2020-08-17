using DiscordNewsBot.Clients;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using DiscordNewsBot.Config;
using DSharpPlus;
using YoutubeExplode.Videos;

namespace DiscordNewsBot.Threading
{
    ///<inheritdoc/>
    internal class YouTubeObserver : BaseObserver
    {
        private static readonly string Tag = typeof(YouTubeObserver).Name;
        protected override void ObservingThread()
        {
            while (true)
            {
                // Wait signal from timer
                WaitHandler.WaitOne();

                // Check if there are any channels to check new content
                Program.Log(Tag, "Checking new videos on channels");
                if (SettingsManager.Config.ChannelList.Count < 1)
                {
                    Program.Log(Tag, "Nothing to check");
                }

                foreach (var chatChannel in SettingsManager.Config.ChannelList)
                {
                    try
                    {
                        // If chat muted, skip it
                        if (SettingsManager.Config.MutedServers.Contains(chatChannel.DiscordChatId))
                            continue;
                        //If not YotubeService, skip it
                        if (chatChannel.ServiceType != typeof(YouTubeClient))
                            continue;

                        var reflectionSingletonGetter = (IClient)chatChannel.ServiceType.GetMethod("GetInstance")?.Invoke(null, null);
                        var youTubeTask = reflectionSingletonGetter?.GetLast(chatChannel.ServiceChannel);
                        youTubeTask?.Wait();
                        var video = (Video)youTubeTask?.Result;

                        // If video found and not already sent to text chat channel
                        if (video != null && reflectionSingletonGetter.AddLast(chatChannel.DiscordChatId, chatChannel.ServiceChannel, video.Id))
                        {
                            Task<DiscordChannel> discordChatTask = Program.Discord.GetChannelAsync(chatChannel.DiscordChatId);
                            discordChatTask.Wait();

                            var notifyMessage =
                                string.Format(
                                    Resources.Resource.ResourceManager.GetString("youtube-notify-text",
                                        discordChatTask.Result.GetCultureInfo()), video.Author, video.Id);
                            Program.Discord.SendMessageAsync(discordChatTask.Result, notifyMessage);
                            Program.Log(Tag, $"New video for YouTube channel <{video.Author}> found. Sending it to discord chat <{discordChatTask.Result.Name}>");
                        } else
                        {
                            Program.Log(Tag, video == null ? $"No video found or it's null" : $"New YouTube video skipped '{video.Title}'", LogLevel.Debug);
                        }
                    }
                    catch (Exception ex)
                    {
                        Program.Log(Tag, ex, DSharpPlus.LogLevel.Error);
                    }
                }
                Program.Log(Tag, $"All channels checked. \nNext check:{DateTime.Now.AddMilliseconds(SettingsManager.Config.ObserveCheckInterval)}");
            }
        }
    }
}
