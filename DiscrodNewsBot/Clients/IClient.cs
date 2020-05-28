using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordNewsBot.Clients
{
    /// <summary>
    /// Interface for some external service client
    /// NOTE: Implement with singleton pattern
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Searches on service
        /// </summary>
        /// <param name="query">Search query</param>
        /// <param name="limit"></param>
        /// <returns>Link(s) to found information/page</returns>
        public abstract Task<List<string>> Search(string query, int limit = 3);
        /// <summary>
        /// Searches information channel on service
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>Link(s) to found information/page</returns>
        public abstract Task<Dictionary<string, string>> SearchChannel(string query);
        /// <summary>
        /// Searches information channel on service
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Information channel dictionary - [name:id]</returns>
        public abstract Task<string> SearchChannelId(string query);
        /// <summary>
        /// Searches new information on service
        /// </summary>
        /// <param name="channelId">Information channel id or link</param>
        /// <returns>New information object</returns>
        public abstract Task<object> GetLast(string channelId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public abstract bool AddLast(ulong chatId, string channelId, string videoId);
    }
}