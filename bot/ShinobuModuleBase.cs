using System;
using System.IO;
using Disqord;
using Disqord.Bot;
using Shinobu.Extensions;

namespace Shinobu
{
    public abstract class ShinobuModuleBase : DiscordModuleBase
    {
        public static long GetTimestamp()
        {
            return Convert.ToInt64((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }
        
        protected DiscordResponseCommandResult EmbedReply(string description)
        {
            return Reply(GetEmbed(description));
        }

        protected DiscordResponseCommandResult Embed(string description)
        {
            return Response(GetEmbed(description));
        }

        protected LocalEmbed GetEmbed(string? description = null)
        {
            var embed = (new LocalEmbed())
                .WithColor(Program.Color);

            if (description != null) {
                embed.WithDescription(description);
            }

            return embed;
        }

        protected DiscordCommandResult RespondWithAttachment(Stream stream)
        {
            return RespondWithAttachment(GetEmbed(), stream);
        }
        
        protected DiscordCommandResult RespondWithAttachment(
            string description,
            Stream stream)
        {
            return RespondWithAttachment(GetEmbed(description), stream);
        }

        protected DiscordCommandResult RespondWithAttachment(
            LocalEmbed embed,
            Stream stream)
        {
            stream.Rewind(); // this would later crash if not rewound so

            return Response(
                (new LocalMessage())
                .WithEmbed(embed
                    .WithImageUrl("attachment://file.png"))
                .AddAttachment(new LocalAttachment(stream, "file.png"))
            );
        }
    }
}