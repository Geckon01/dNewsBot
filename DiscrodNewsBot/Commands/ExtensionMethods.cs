using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordNewsBot.Config
{
    internal static partial class ExtensionMethods
    {
        /// <summary>
        /// Split list to chunks 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="nSize">Size of chunk</param>
        /// <returns></returns>
        public static IEnumerable<List<T>> FastSplit<T>(this List<T> list, int parts = 30)
        {
            for (int i = 0; i < list.Count; i += parts)
            {
                yield return list.GetRange(i, Math.Min(parts, list.Count - i));
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            int i = 0;
            var splits = from item in list
                group item by i++ % parts into part
                select part.AsEnumerable();
            return splits;
        }
    }
}