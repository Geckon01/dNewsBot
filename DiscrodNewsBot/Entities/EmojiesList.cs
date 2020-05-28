using System.Collections.Generic;
using System.Text;

namespace DiscordNewsBot.Entities
{
    class EmojiesList
    {
        /// <summary>
        /// Numeric emojies from 0 to 9
        /// </summary>
        public static string[] Numbers =
        {
            ":zero:",
            ":one:",
            ":two:",
            ":three:",
            ":four:",
            ":five:",
            ":six:",
            ":seven:",
            ":eight:",
            ":nine:"
        };

        /// <summary>
        /// Split any integer to array of numbers
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static int[] GetArrayFormNumber(int num)
        {
            List<int> listOfInts = new List<int>();
            while (num > 0)
            {
                listOfInts.Add(num % 10);
                num = num / 10;
            }
            listOfInts.Reverse();
            return listOfInts.ToArray();
        }

        /// <summary>
        /// Get emojie number from integer
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetEmojiNumFromInt(int num)
        {
            if (num < 9)
                return Numbers[num];

            StringBuilder builder = new StringBuilder();
            foreach (var intNumber in GetArrayFormNumber(num))
            {
                builder.Append(Numbers[intNumber]);
            }

            return builder.ToString();
        }
    }
}
