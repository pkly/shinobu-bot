using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace Shinobu.Commands
{
    public static class Dynamic
    {
        public static async Task DoReaction(DiscordCommandContext context)
        {
            IEnumerable<IMember?> members = ((IMember?[]) context.Arguments[0]).Where(x => x != null);
            var mentions = "";
            foreach (var member in members)
            {
                mentions += " and " + member.Mention;
            }
            
            Console.WriteLine(mentions);
            await new DiscordResponseCommandResult(context, (new LocalMessageBuilder()).WithContent("yeet " + mentions).Build());
        }
    }
}