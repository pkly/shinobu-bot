using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Rest;
using Qmmands;
using Shinobu.Attributes;
using Shinobu.Extensions;

namespace Shinobu.Commands
{
    [Section("Utility")]
    public class Utility : ShinobuModuleBase
    {
        private const string PING_MESSAGE = "Receive delay {0}ms, latency is {1}ms";

        private const string INVITE_TEXT = "**[Click this link]({0}) to invite me!**\nJoin our [support server]({1})!";
        private const string INVITE_URL = "https://discord.com/oauth2/authorize?client_id=490901986502377512&scope=bot&permissions=388160";
        private const string SUPPORT_SERVER = "https://discord.gg/qwdMmsG/";

        private readonly CommandService _commands;

        public Utility(CommandService commands)
        {
            _commands = commands;
        }
        
        [Command("ping")]
        public async Task Ping()
        {
            long message = Context.Message.CreatedAt.ToUnixTimeMilliseconds();
            long diff = GetTimestamp() - message;
            var embed = GetEmbed(String.Format(PING_MESSAGE, diff, '?'));
            var response = await Response(embed);
            await response.ModifyAsync(x => 
                x.Embed = embed.WithDescription(
                    String.Format(PING_MESSAGE, diff, response.CreatedAt.ToUnixTimeMilliseconds() - message)
                ).Build()
            );
        }

        [Command("owner")]
        [RequireBotOwner]
        public DiscordCommandResult IsOwner()
        {
            return Embed(
                string.Format(
                    "Yes, {0} is a bot owner!",
                    Context.Author.Mention
                )
            );
        }
        
        [Command("avatar", "pfp", "image", "profilepic", "pic")]
        [RequireGuild]
        public DiscordCommandResult Avatar(IMember? member = null)
        {
            member ??= Context.GetCurrentMember();

            return Reply(
                GetEmbed()
                    .WithTitle(member.NickOrName() + "'s avatar")
                    .WithImageUrl(member.GetAvatarUrl(ImageFormat.Default, 256))
            );
        }
        
        [Command("emote", "emoji", "enlarge", "steal")]
        public DiscordCommandResult Emote(ICustomEmoji emoji)
        {
            return Reply(
                GetEmbed()
                    .WithImageUrl(emoji.GetUrl())
                    .WithTitle(emoji.Name ?? "No emote name")
                    .WithFooter("ID: " + emoji.Id.ToString() + string.Format(", {0}", emoji.IsAnimated ? "Animated (.gif)" : "Image (.png)"))
                    .WithUrl(emoji.GetUrl())
            );
        }

        [Command("help")]
        public async Task Help()
        {
            var embeds = new List<LocalEmbedBuilder>();

            var commands = new Dictionary<string, List<Command>>();
            var sections = new Dictionary<string, SectionAttribute>();
            
            // categorize commands by section first
            foreach (var command in _commands.GetAllCommands())
            {
                bool foundSection = false;
                foreach (var attribute in command.Attributes)
                {
                    if (attribute is SectionAttribute attr)
                    {
                        if (!commands.ContainsKey(attr.Name))
                        {
                            commands.Add(attr.Name, new List<Command>());
                            sections.Add(attr.Name, attr);
                        }
                        
                        commands[attr.Name].Add(command);
                        foundSection = true;
                    }
                }

                if (foundSection)
                {
                    continue;
                }

                foreach (var attribute in command.Module.Attributes)
                {
                    if (attribute is SectionAttribute attr)
                    {
                        if (!commands.ContainsKey(attr.Name))
                        {
                            commands.Add(attr.Name, new List<Command>());
                            sections.Add(attr.Name, attr);
                        }
                        
                        commands[attr.Name].Add(command);
                    }
                }
            }

            var last = sections.Last();
            foreach (var attributePair in sections)
            {
                var embed = GetEmbed()
                    .WithTitle(attributePair.Value.Name);

                var description = "";
                if (attributePair.Value.Description == null)
                {
                    description = attributePair.Value.Description + "\n=======================\n\n";
                }
                
                foreach (var command in commands[attributePair.Value.Name])
                {
                    description += command.FullAliases[0] + " " + string.Join<string>(' ', command.Parameters.Select<Parameter, string>(new Func<Parameter, string>(FormatParameter))) + "\n";
                }

                // remove last newlines
                embed.WithDescription(description.Substring(0, description.Length - 1));

                if (attributePair.Equals(last))
                {
                    embed.WithFooter("Made by Ly#3449, original concept by zappin#1312, version " + Program.Version);
                }
                
                embeds.Add(embed);
            }
            
            var builder = new LocalMessageBuilder();
            var lastEmbed = embeds.Last();
            var addedReaction = false;
            foreach (var embed in embeds)
            {
                try
                {
                    await Context.Author.SendMessageAsync(builder.WithEmbed(embed).Build());
                    if (!embed.Equals(lastEmbed))
                    {
                        await Task.Delay(500);
                    }

                    if (!addedReaction)
                    {
                        await Context.Message.AddReactionAsync(new LocalEmoji("✅"));
                    }
                }
                catch (Exception e) // in case someone has blocked dms
                {
                    await EmbedReply("You seem to have dms disabled or other error occurred!");
                    return;
                }
            }
            
            // this is just pulled out of Disqord.Bot.FormatFailureMessage
            static string FormatParameter(Parameter parameter)
            {
                string format;
                if (parameter.IsMultiple)
                {
                    format = "{0}[]";
                }
                else
                {
                    string str = parameter.IsRemainder ? "{0}…" : "{0}";
                    format = parameter.IsOptional ? "[" + str + "]" : "<" + str + ">";
                }
                return string.Format(format, (object) parameter.Name);
            }
        }

        [Command("invite", "inv")]
        public DiscordCommandResult Invite()
        {
            return Reply(
                GetEmbed(string.Format(
                    INVITE_TEXT,
                    INVITE_URL,
                    SUPPORT_SERVER
                ))
                    .WithFooter(string.Format(
                        "Currently in {0} servers!",
                        Context.Bot.GetGuilds().Count
                    ))
            );
        }
    }
}