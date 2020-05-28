using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using DiscordNewsBot.Entities;
using DSharpPlus.Entities;

namespace DiscordNewsBot.Config
{
    internal static partial class ExtensionMethods
    {
        /// <summary>
        /// Check if chat channel list contains key
        /// </summary>
        /// <param name="chatChannels"></param>
        /// <param name="id">Text chat channel id</param>
        /// <returns></returns>
        public static bool ContainsKey(this List<ChatChannel> chatChannels, ulong id)
        {
            var result = false;
            foreach (var chatChannel in chatChannels)
            {
                if (chatChannel.DiscordChatId == id)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Check if chat channel list contains channel
        /// </summary>
        /// <param name="chatChannels"></param>
        /// <param name="chatId">Text chat channel id</param>
        /// <param name="channelName">Text channel name</param>
        /// <returns></returns>
        public static ChatChannel ContainsChannel(this List<ChatChannel> chatChannels, ulong chatId, string channelName)
        {
            foreach (var chatChannel in chatChannels)
            {
                if (chatChannel.DiscordChatId == chatId && chatChannel.ServiceChannel == channelName)
                    return chatChannel;
            }

            return null;
        }
        
        /// <summary>
        /// Get text channel culture if provided
        /// </summary>
        /// <param name="chatChannel"></param>
        /// <returns>Text channel culture or default culture if not provided</returns>
        public static CultureInfo GetCultureInfo(this DiscordChannel chatChannel)
        {
            return SettingsManager.Config.ChatCultures.ContainsKey(chatChannel.Id) ? SettingsManager.Config.ChatCultures[chatChannel.Id] : Thread.CurrentThread.CurrentCulture;
        }
    }
}
