﻿using System;
using System.Collections.Generic;
using System.IO;

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

        public static void Rewind(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        public static bool NextBoolean(this Random random)
        {
            return Convert.ToBoolean(random.Next(2)); // why 2 lmao C#
        }

        public static string ToReadable(this TimeSpan span)
        {
            var total = span.TotalSeconds;
            var hours = Convert.ToUInt32(Math.Floor(total / 3600));
            total %= 3600;
            var minutes = Convert.ToUInt32(Math.Floor(total / 60));
            var seconds = Convert.ToUInt32(Math.Floor(total % 60));

            return hours.ToString().PadLeft(2, '0') + ':' +
                   minutes.ToString().PadLeft(2, '0') + ':' +
                   seconds.ToString().PadLeft(2, '0');
        }
    }
}