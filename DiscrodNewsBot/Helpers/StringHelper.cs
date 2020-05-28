using System.Security.Cryptography;
using System.Text;

namespace DiscordNewsBot.Helpers
{
    public class StringHelper
    {
        /// <summary>
        /// Get md5 hash string from any string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string CalcHashMd5(string inputString)
        {
            using (MD5 sha256 = MD5.Create())
            {
                byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString));
                var builder = new StringBuilder(data.Length);
                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
