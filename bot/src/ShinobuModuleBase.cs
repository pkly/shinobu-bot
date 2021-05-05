using System.IO;
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

        protected LocalEmbedBuilder GetEmbed(string? description = null)
        {
            var embed = (new LocalEmbedBuilder())
                .WithColor((Color) System.Drawing.ColorTranslator.FromHtml(Helper.Env("EMBED_COLOR")));

            if (description != null) {
                embed.WithDescription(description);
            }

            return embed;
        }
        
        protected DiscordCommandResult RespondWithAttachment(
            string description,
            Stream stream)
        {
            return RespondWithAttachment(GetEmbed(description), stream);
        }

        protected DiscordCommandResult RespondWithAttachment(
            LocalEmbedBuilder embed,
            Stream stream)
        {
            stream.Rewind(); // this would later crash if not rewound so

            return Response(
                (new LocalMessageBuilder())
                .WithEmbed(embed
                    .WithImageUrl("attachment://file.png"))
                .AddAttachment(new LocalAttachment(stream, "file.png"))
                .Build()
            );
        }
    }
}