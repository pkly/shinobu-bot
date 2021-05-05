using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Shinobu.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Shinobu.Commands
{
    public static class Dynamic
    {
        public static async Task DoReaction(DiscordCommandContext context)
        {
            var client = context.Services.GetService<HttpClient>();
            IMember?[] members = ((IMember?[]) context.Arguments[0]).Where(x => x != null).ToArray();
            
            var embed = (new LocalEmbedBuilder())
                .WithColor((Color) System.Drawing.ColorTranslator.FromHtml(Program.Env("EMBED_COLOR")));
            var builder = new LocalMessageBuilder();

            var apiCommand = Program.ApiCommands[context.Command.Name];

            // for now emulate the old behavior
            var text = apiCommand.NoMention.Random();
            if (members.Length > 0)
            {
                text = apiCommand.Mention.Random();
            }
            
            // insert the author as the first "mention"
            members = members.Prepend((IMember) context.Author).ToArray();

            // insert mentions
            if (text != null)
            {
                text = string.Format(text, members.Select(x => x.Mention).ToArray());
                embed.WithDescription(text);
            }
            
            var response = await client.GetAsync(apiCommand.Url);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                await new DiscordResponseCommandResult(context, builder.WithEmbed(embed.WithDescription("An error occurred while fetching reaction!")).Build());
                return;
            }

            // decode the json response, nothing else is supported for the time being
            // we also only care for one level of the response as of now.
            var decoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());
            
            embed.WithImageUrl(apiCommand.GetFixedImageUrl(decoded[apiCommand.Path]));
            await new DiscordResponseCommandResult(context, (new LocalMessageBuilder()).WithEmbed(embed).Build());
        }
    }
}