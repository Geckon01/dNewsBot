using DiscordNewsBot.Commands;
using DiscordNewsBot.Threading;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using System;
using System.Threading.Tasks;
using DiscordNewsBot.Config;
using DSharpPlus.Interactivity;

namespace DiscordNewsBot
{
    internal class Program
    {

        /// <summary>
        /// Discord DSharpPlus client
        /// </summary>
        public static DiscordClient Discord { get; private set; }

        /// <summary>
        /// Program start time
        /// </summary>
        public static DateTime StartTime { get; private set; }

        /// <summary>
        /// YouTubeServer observer
        /// </summary>
        public static YouTubeObserver YouTubeObserver { get; private set; }

        /// <summary>
        /// DSharPLusComandsModule
        /// </summary>
        private static CommandsNextModule _commands;

        /// <summary>
        /// App tag for logging
        /// </summary>
        private static readonly string Tag = typeof(Program).Name;

        [STAThread]
        private static void Main(string[] args)
        {
            //Capture start time and start
            StartTime = DateTime.Now;

            AppDomain currentDomain = default(AppDomain);
            currentDomain = AppDomain.CurrentDomain;
            // Handler for unhandled exceptions.
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;

            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Main program method
        /// </summary>
        /// <returns></returns>
        private static async Task MainAsync()
        {
            Discord = new DiscordClient(new DiscordConfiguration
            {
                Token = SettingsManager.Config.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Info
            });
            
            Discord.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith(SettingsManager.Config.CommandPrefix))
                {
                    await Discord.UpdateStatusAsync(null, DSharpPlus.Entities.UserStatus.Online);
                }
            };
            _commands = Discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = SettingsManager.Config.CommandPrefix
            });

            Discord.UseInteractivity(new InteractivityConfiguration
            {
                // default pagination behaviour to just ignore the reactions
                PaginationBehaviour = TimeoutBehaviour.Ignore,

                // default pagination timeout to 5 minutes
                PaginationTimeout = TimeSpan.FromMinutes(5),

                // default timeout for other actions to 2 minutes
                Timeout = TimeSpan.FromMinutes(2)
            });
         
            _commands.RegisterCommands<MainCommands>();
            _commands.RegisterCommands<LimitingCommands>();
            _commands.RegisterCommands<OwnerCommands>();
            _commands.RegisterCommands<AdminCommands>();
            _commands.RegisterCommands<YotubeCommands>();
            
            await Discord.ConnectAsync();

            //Creating all the observers that will check for updated on specific service
            YouTubeObserver = new YouTubeObserver();
            YouTubeObserver.Start();

            await Task.Delay(-1);
        }

        /// <summary>
        /// Wrap for logging some message
        /// </summary>
        /// <param name="application">Some context</param>
        /// <param name="message">Message to log</param>
        /// <param name="level">Log level from DSharpPlus</param>
        public static void Log(string application, string message, LogLevel level = LogLevel.Info)
        {
            Discord?.DebugLogger.LogMessage(level, application, message, DateTime.Now);
        }

        /// <summary>
        /// Wrap for logging some error
        /// </summary>
        /// <param name="application">Some context</param>
        /// <param name="ex">Exception to log</param>
        /// <param name="level">Log level from DSharpPlus</param>
        public static void Log(string application, Exception ex, LogLevel level = LogLevel.Info)
        {
            Discord?.DebugLogger.LogMessage(level, application, $"{ex.Message}, Stack trace: \n{ex.StackTrace}", DateTime.Now);
        }

        /// <summary>
        /// Awaking all the observers threads
        /// </summary>
        public static void ForceCheck()
        {
            YouTubeObserver.Awake();
        }

        /// <summary>
        /// Shutting down the bot properly by saving all cached values
        /// </summary>
        /// <returns></returns>
        public static async Task Shutdown()
        {
            SettingsManager.Config.Save();

            Log(Tag, "Shutdown command received, now shutting down!");
            await Discord.UpdateStatusAsync(null, DSharpPlus.Entities.UserStatus.Offline);
            await Discord.DisconnectAsync();
            Environment.Exit(0);
        }

        /// <summary>
        /// Handler for unhalder exceptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = default(Exception);
            ex = (Exception)e.ExceptionObject;
            if(Discord != null)
               Program.Log(typeof(Program).Name, ex, LogLevel.Error);
            else 
                Console.WriteLine($"[{DateTime.Now.ToString()}]App crash erro: {ex.Message} /n Stack trace: {ex.StackTrace}");
        }
    }
}
