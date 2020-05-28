using System;

namespace DiscordNewsBot.Entities
{
    class ChatChannel
    {
        /// <summary>
        /// Discord text channel id
        /// </summary>
        public ulong DiscordChatId;
        /// <summary>
        /// Notification service channel
        /// </summary>
        public string ServiceChannel;
        /// <summary>
        /// Notification service type
        /// </summary>
        public Type ServiceType;
        /// <summary>
        /// Notification service name
        /// </summary>
        public string ServiceChannelName;

        public ChatChannel() { }
        public ChatChannel(ulong discordChat, string serviceChannel, Type serviceType, string serviceChannelName)
        {
            DiscordChatId = discordChat;
            ServiceChannel = serviceChannel;
            ServiceType = serviceType;
            ServiceChannelName = serviceChannelName;
        }
    }
}
