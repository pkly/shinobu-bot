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
        /// <summary>
        /// Handles simple Http-apis for "reaction" commands
        ///
        /// The incoming argument 0 from context is a collection of IMember, but for later purposes
        /// that type is not required, as we simply use IUser's Mention property, as such converting
        /// down to the lower interface acceptable to allow for implicit targetting
        /// </summary>
        /// <param name="context"></param>
        public static async Task DoReaction(DiscordCommandContext context)
        {
            var client = context.Services.GetRequiredService<HttpClient>();
            List<IUser> users = ((IUser?[]) context.Arguments[0]).Where(x => x != null).ToList()!;
            
            var embed = (new LocalEmbed())
                .WithColor(Program.Color);
            var builder = new LocalMessage();
            var apiCommand = Program.ApiCommands[context.Command.Name];

            // if no members are specified allow for implicit targeting via the reply system
            if (users.Count == 0 &&
                context.Message.ReferencedMessage.HasValue &&
                context.Message.ReferencedMessage.Value != null &&
                context.Message.ReferencedMessage.Value.Author != null)
            {
                users.Add(context.Message.ReferencedMessage.Value.Author);
            }
            
            // prepare reaction text if needed
            string? text = null;
            if (apiCommand.Actions.Ranges.Count > 0)
            {
                text = apiCommand.Actions.GetValue(users.Count)?.Random();
            }

            // insert the author as the first "mention"
            users = users.Prepend(context.Author).ToList();

            // insert mentions
            if (text != null)
            {
                text = string.Format(text, users.Select(x => x.Mention).ToArray());
                embed.WithDescription(text);
            }
            
            var response = await client.GetAsync(apiCommand.Url);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                await new DiscordResponseCommandResult(context, builder.WithEmbed(embed.WithDescription("An error occurred while fetching reaction!")));
                return;
            }

            // decode the json response, nothing else is supported for the time being
            // we also only care for one level of the response as of now.
            var decoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());
            
            embed.WithImageUrl(apiCommand.GetFixedImageUrl(decoded[apiCommand.Path]));
            await new DiscordResponseCommandResult(context, (new LocalMessage()).WithEmbed(embed));
        }
    }
}