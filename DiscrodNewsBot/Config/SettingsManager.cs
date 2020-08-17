using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DiscordNewsBot.Entities;
using Newtonsoft.Json;

namespace DiscordNewsBot.Config
{
    /// <summary>
    /// Stores all bot settings
    /// </summary>
    internal class SettingsManager
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        [NonSerialized]
        private static SettingsManager _instance;
        /// <summary>
        /// Path to json config file
        /// </summary>
        [NonSerialized]
        private const string ConfigFilePath = "./Config/config.json";

        /// <summary>
        /// Discord token to access bot app
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// DiscordBotsList token
        /// </summary>
        public string TopGgToken { get; set; }
        /// <summary>
        /// Maximum time when any observing service information is relevant
        /// </summary>
        public int TimeToNotRelevant { get; set; }
        /// <summary>
        /// Bot command prefix
        /// </summary>
        public string CommandPrefix { get; set; }
        /// <summary>
        /// App client_id.
        /// Is used for bot invite link
        /// </summary>
        public ulong ClientId { get; set; }
        /// <summary>
        /// Bot permissions integer.
        /// Is used for bot invite link
        /// </summary>
        public uint BotPermissions { get; set; }
        /// <summary>
        /// Saved observing channels list
        /// </summary>
        public List<ChatChannel> ChannelList { get; set; }
        /// <summary>
        /// Muted discord chats.
        /// Will not receive any notification
        /// </summary>
        public List<ulong> MutedServers { get; set; }
        /// <summary>
        /// Interval for checking new info on services in ms
        /// </summary>
        public int ObserveCheckInterval { get; set; }
        /// <summary>
        /// Some discord chat selected cultures
        /// </summary>
        public Dictionary<ulong, CultureInfo> ChatCultures { get; set; }

        public List<CultureInfo> SupportedCultures { get; set; }

        /// <summary>
        /// Private constructor due to singleton pattern
        /// </summary>
        private SettingsManager() { }

        /// <summary>
        /// Returns actual bot config
        /// </summary>
        public static SettingsManager Config
        {
            get
            {
                if (_instance == null)
                {
                    SettingsManager.Load();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Stores config to json file on disk
        /// </summary>
        public void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_instance);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                Program.Log("ERROR", $"{ex.Message} at {typeof(SettingsManager)}", DSharpPlus.LogLevel.Error);
            }
        }

        /// <summary>
        /// Loads config from json file on disk
        /// </summary>
        public static void Load()
        {
            try
            {
                var jsonConfig = File.ReadAllText(ConfigFilePath);
                _instance = JsonConvert.DeserializeObject<SettingsManager>(jsonConfig);
            }
            catch (Exception ex)
            {
                Program.Log("ERROR", $"{ex.Message} at {typeof(SettingsManager)}", DSharpPlus.LogLevel.Error);
            }
        }
    }
}
