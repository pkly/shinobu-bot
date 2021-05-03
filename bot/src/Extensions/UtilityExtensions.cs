using System;
using System.Collections.Generic;

namespace Shinobu.Extensions
{
    public static class UtilityExtensions
    {
        private static Random _random = new Random();

        public static T? Random<T>(this T[] items)
        {
            if (items.Length == 0)
            {
                return default;
            }
            
            return items[_random.Next(items.Length)];
        }

        public static T? Random<T>(this IList<T> items)
        {
            if (items.Count == 0)
            {
                return default;
            }
            
            return items[_random.Next(items.Count)];
        }
    }
}