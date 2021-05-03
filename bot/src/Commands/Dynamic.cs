using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace Shinobu.Commands
{
    public static class Dynamic
    {
        public static async Task<DiscordResponseCommandResult> DoReaction(DiscordCommandContext context, CommandService commands)
        {
            List<IMember> members = new List<IMember>();
            Console.WriteLine(context.Arguments.Count);
            if (context.Arguments.Count > 0 && context.Arguments[0] != null && !string.IsNullOrEmpty(context.Arguments[0].ToString()))
            {
                foreach (var arg in context.Arguments[0].ToString().Split(" "))
                {
                    var user = await commands.GetTypeParser<IMember>().ParseAsync(null, arg, context);
                    if (user.IsSuccessful)
                    {
                        members.Add(user.Value);
                    }
                }
            }

            var mentions = "";
            foreach (var member in members)
            {
                mentions += " and " + member.Mention;
            }
            
            Console.WriteLine(mentions);
            return new DiscordResponseCommandResult(context, (new LocalMessageBuilder()).WithContent("yeet " + mentions).Build());
        }
    }
}