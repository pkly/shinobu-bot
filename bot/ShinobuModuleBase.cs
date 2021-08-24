﻿using System.IO;
using Disqord;
using Disqord.Bot;
using Shinobu.Extensions;

namespace Shinobu
{
    public abstract class ShinobuModuleBase : DiscordModuleBase
    {
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
                .WithEmbeds(embed
                    .WithImageUrl("attachment://file.png"))
                .AddAttachment(new LocalAttachment(stream, "file.png"))
            );
        }

        protected DiscordCommandResult ReplyWithAttachment(Stream stream)
        {
            return ReplyWithAttachment(GetEmbed(), stream);
        }

        protected DiscordCommandResult ReplyWithAttachment(
            string description,
            Stream stream)
        {
            return ReplyWithAttachment(GetEmbed(description), stream);
        }
        
        protected DiscordCommandResult ReplyWithAttachment(
            LocalEmbed embed,
            Stream stream)
        {
            stream.Rewind(); // this would later crash if not rewound so

            return Reply(
                (new LocalMessage())
                .WithEmbeds(embed
                    .WithImageUrl("attachment://file.png"))
                .AddAttachment(new LocalAttachment(stream, "file.png"))
            );
        }
    }
}