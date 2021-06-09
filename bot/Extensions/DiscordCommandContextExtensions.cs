using Disqord;
using Disqord.Bot;

namespace Shinobu.Extensions
{
    public static class DiscordCommandContextExtensions
    {
        public static IMember? GetCurrentMember(this DiscordCommandContext context)
        {
            if (context.Author == null || context.GuildId == null)
            {
                return null;
            }

            return (IMember) context.Author;
        }
    }
}