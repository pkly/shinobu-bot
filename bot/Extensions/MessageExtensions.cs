using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Disqord;

namespace Shinobu.Extensions
{
    public static class MessageExtensions
    {
        private const string EMOJI_REGEX = "<(a)?:(.*?):(.*?)>";
        
        public static IEnumerable<ICustomEmoji> GetCustomEmoji(this IUserMessage message)
        {
            var list = new List<ICustomEmoji>();
            foreach (Match match in Regex.Matches(message.Content, EMOJI_REGEX))
            {
                try
                {
                    list.Add(new LocalCustomEmoji(
                        new Snowflake(Convert.ToUInt64(match.Groups[3].Value)),
                        match.Groups[2].Value,
                        match.Groups[1].Value.Length > 0
                    ));
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return list;
        }
    }
}