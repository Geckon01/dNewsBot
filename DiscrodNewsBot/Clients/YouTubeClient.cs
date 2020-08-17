using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordNewsBot.Config;
using YoutubeExplode;
using DiscordNewsBot.Helpers;
using DSharpPlus;
using YoutubeExplode.Videos;

namespace DiscordNewsBot.Clients
{
    ///<inheritdoc/>
    internal class YouTubeClient : IClient
    {
        private static YouTubeClient _instance;
        private static List<string> _lastVideoList;
        private readonly YoutubeClient _youtubeService;

        private YouTubeClient()
        {
            _lastVideoList = new List<string>();
            _youtubeService = new YoutubeClient();
        }

        public static IClient GetInstance()
        {
            return _instance ??= new YouTubeClient();
        }

        public bool AddLast(ulong chatId, string channelId, string videoId)
        {
            try
            {
                var hashString = StringHelper.CalcHashMd5(chatId + channelId + videoId);
                if (!_lastVideoList.Contains(hashString))
                {
                    if (_lastVideoList.Count > 99)
                        _lastVideoList.RemoveRange(0, 50);
                    _lastVideoList.Add(hashString);
                    return true;
                }
            }
            catch (Exception e)
            {
                Program.Log(typeof(YouTubeClient).Name, e, LogLevel.Critical);
                return false;
            }
               
            return false;
        }
        
        public async Task<List<string>> Search(string query, int limit = 3)
        {
            var searchResult = await _youtubeService.Search.GetVideosAsync(query);
            var videosBuilder = new List<string>();

            limit = limit < searchResult.Count ? limit : searchResult.Count;
            for (var i = 0; i < limit; i++)
            {
                videosBuilder.Add(string.Format("{0} — https://www.youtube.com/watch?v={1};{2}{3}{3}", searchResult[i].Title, searchResult[i].Id, searchResult[i].Thumbnails.StandardResUrl, Environment.NewLine));
            }
            return videosBuilder;
        }
        public async Task<Dictionary<string, string>> SearchChannel(string query)
        {
            var videoSearchResult = await _youtubeService.Search.GetVideosAsync(query, 0, 1);
            var channelList = new Dictionary<string, string>();

            var foundChannel = await _youtubeService.Channels.GetByVideoAsync(videoSearchResult[0].Id);
            if(!channelList.ContainsKey(foundChannel.Title))
                channelList.Add(foundChannel.Title, foundChannel.Id);

            videoSearchResult = null;
            foundChannel = null;
            GC.Collect();
            return channelList;
        }

        public async Task<string> SearchChannelId(string query)
        {
            IReadOnlyList<Video> foundVideo = await _youtubeService.Search.GetVideosAsync(query);
            foreach (var item in foundVideo)
            {
                if (item.Author.Contains(query))
                    return item.ChannelId;
            }
            return "";
        }

        public async Task<object> GetLast(string channelId)
        {
            var videosList = await _youtubeService.Channels.GetUploadsAsync(channelId);
            if (videosList.Count <= 0) return null;
            var uploadDelta = DateTime.Now - videosList[0].UploadDate;
            return uploadDelta.Days < 1 && uploadDelta.Hours < SettingsManager.Config.TimeToNotRelevant ? videosList[0] : null;
        }
    }
}
