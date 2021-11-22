using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Rest;
using MoreLinq;
using Qmmands;
using Shinobu.Attributes;
using Shinobu.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;

namespace Shinobu.Commands
{
    [Section("Utility")]
    public class Utility : ShinobuModuleBase
    {
        private const string PING_MESSAGE = "Receive delay {0}ms, latency is {1}ms";

        private const string INVITE_TEXT = "**[Click this link]({0}) to invite me!**\nJoin our [support server]({1})!";
        private const string INVITE_URL = "https://discord.com/oauth2/authorize?client_id=490901986502377512&scope=bot&permissions=388160";
        private const string SUPPORT_SERVER = "https://discord.gg/qwdMmsG/";
        private const string CHANGELOG_URL = "https://raw.githubusercontent.com/pkly/shinobu-bot/master/CHANGELOG.md";

        private const string STATUS_MESSAGE = "Uptime: **{0}**, servers: **{1}**, v{2}";

        private readonly CommandService _commands;
        private readonly HttpClient _client;

        private string? _changelog;

        public Utility(CommandService commands, HttpClient client)
        {
            _commands = commands;
            _client = client;
        }
        
        [Command("ping")]
        [Description("Check delay between the bot and api")]
        public async Task Ping()
        {
            long message = Context.Message.CreatedAt().ToUnixTimeMilliseconds();
            long diff = Program.Timestamp - message;
            var embed = Program.GetEmbed(String.Format(PING_MESSAGE, diff, '?'));
            var response = await Response(embed);
            await response.ModifyAsync(x => 
                x.Embeds = new LocalEmbed[] {
                    embed.WithDescription(
                        String.Format(PING_MESSAGE, diff, response.CreatedAt().ToUnixTimeMilliseconds() - message)
                    )
                }
            );
        }

        [RequireBotOwner]
        [HiddenCommand]
        [Command("owner")]
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
        [Description("Show a user's profile picture")]
        public DiscordCommandResult Avatar(IMember? member = null)
        {
            member ??= Context.GetCurrentMember();

            return Reply(
                Program.GetEmbed()
                    .WithTitle(member!.NickOrName() + "'s avatar")
                    .WithImageUrl(member.GetAvatarUrl(CdnAssetFormat.Automatic, 256))
            );
        }

        [Command("banner")]
        [Description("Show a user's profile banner (or color)")]
        public async Task<DiscordCommandResult> Banner(IMember? member = null)
        {
            member ??= Context.GetCurrentMember();
            
            // re-fetch user cuz otherwise no banner
            // also this crashes if you fetch a member lmao
            var user = await Context.Bot.FetchUserAsync(member!.Id);

            var embed = Program.GetEmbed()
                .WithTitle(member!.NickOrName() + "'s banner");

            string? url = user.GetBannerUrl(CdnAssetFormat.Automatic, 512);
            if (url == null && user!.AccentColor == null)
            {
                return Reply("User has no banner or accent color");
            }
            
            if (url != null)
            {
                return Reply(embed.WithImageUrl(url));
            }
            else
            {
                var stream = new MemoryStream();
                using (Image empty = new Image<Rgba32>(512, 200))
                {
                    empty.Mutate(x => x.Fill(Color.ParseHex(user.AccentColor.ToString()!)));
                    await empty.SaveAsPngAsync(stream);
                }

                return ReplyWithAttachment(embed, stream);
            }
        }
        
        [Command("emote", "emoji", "enlarge", "steal")]
        [Description("Enlarge an emote with a download link... for reasons...")]
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public DiscordCommandResult Emote(ICustomEmoji? emoji = null)
        {
            // if not found, see if this is a reply and try to get it from there
            if (emoji == null &&
                Context.Message.ReferencedMessage.HasValue &&
                Context.Message.ReferencedMessage.Value != null)
            {
                var list = Context.Message.ReferencedMessage.Value.GetCustomEmoji();
                if (list.Any())
                {
                    emoji = list.First();
                }
            }

            if (emoji == null)
            {
                return EmbedReply("No emote found");
            }
            
            return Reply(
                Program.GetEmbed()
                    .WithImageUrl(emoji.GetUrl())
                    .WithTitle(emoji.Name ?? "No emote name")
                    .WithFooter("ID: " + emoji.Id + string.Format(", {0}", emoji.IsAnimated ? "Animated (.gif)" : "Image (.png)"))
                    .WithUrl(emoji.GetUrl())
            );
        }

        /// <summary>
        /// In theory the result of this method could be cached, but it's not a huge issue
        /// and later down the line it's likely dynamic commands will be added so a cache-reset
        /// must be possible anyway
        ///
        /// For the time being don't bother, afaik it's not a huge issue
        /// </summary>
        [Command("help")]
        [Description("Displays this message")]
        public async Task Help()
        {
            var embeds = new List<LocalEmbed>();

            // normal proper commands
            var commands = new Dictionary<string, List<Tuple<Command, Dictionary<Type, Attribute>>>>();
            // section splits
            var sections = new Dictionary<string, SectionAttribute>();
            // extremely simple commands which will not display a description
            // and share a single row for display
            var simpleCommands = new Dictionary<SectionAttribute, List<Command>>();

            // what we'll be collecting from commands
            var searchAttributes = new List<Type>()
            {
                typeof(SectionAttribute),
                typeof(SimpleCommandAttribute)
            };
            
            // categorize commands by section first
            foreach (var command in _commands.GetAllCommands())
            {
                var attrs = new Dictionary<Type, Attribute>();
                var copy = new List<Type>(searchAttributes);

                var attrLists = new List<List<Attribute>>()
                {
                    command.Attributes.ToList(),
                    command.Module.Attributes.ToList()
                };

                var hideCommand = false;
                foreach (var list in attrLists)
                {
                    foreach (var attr in list)
                    {
                        if (attr.GetType() == typeof(HiddenCommandAttribute))
                        {
                            hideCommand = true;
                            break;
                        }
                        
                        for (int i = copy.Count - 1; i > -1; i--)
                        {
                            var item = copy[i];
                            if (attr.GetType() == item)
                            {
                                attrs.Add(item, attr);
                                copy.RemoveAt(i);
                            }
                        }
                    }

                    if (hideCommand)
                    {
                        break;
                    }
                }

                if (hideCommand || !attrs.TryGetValue(typeof(SectionAttribute), out var temp))
                {
                    continue; // invalid or hidden command?
                }

                // re-cast
                SectionAttribute section = (SectionAttribute) temp;
                if (!commands.ContainsKey(section.Name))
                {
                    commands.Add(section.Name, new List<Tuple<Command, Dictionary<Type, Attribute>>>());
                    sections.Add(section.Name, section);
                }
                
                if (!attrs.ContainsKey(typeof(SimpleCommandAttribute)))
                {
                    commands[section.Name].Add(new Tuple<Command, Dictionary<Type, Attribute>>(command, attrs));
                }
                else
                {
                    if (!simpleCommands.ContainsKey(section))
                    {
                        simpleCommands.Add(section, new List<Command>());
                    }
                    
                    simpleCommands[section].Add(command);
                }
            }

            var last = sections.Last();
            foreach (var attributePair in sections)
            {
                var embed = Program.GetEmbed()
                    .WithTitle(attributePair.Value.Name);
                
                var description = "\n";
                if (attributePair.Value.Description != null)
                {
                    description = attributePair.Value.Description + "\n\n=======================\n\n";
                }
                
                // normal commands
                if (commands[attributePair.Value.Name].Count > 0)
                {
                    foreach (var tuple in commands[attributePair.Value.Name])
                    {
                        var name = "**" + tuple.Item1.FullAliases[0] + "**";
                    
                        description += name + " " + string.Join(' ', 
                            tuple.Item1.Parameters.Select<Parameter, string>(FormatParameter)
                        ) + "\n" + (string.IsNullOrEmpty(tuple.Item1.Description) ? "" : "> " + tuple.Item1.Description + "\n\n");
                    }
                }
                
                // simple commands
                if (simpleCommands.TryGetValue(attributePair.Value, out var simpleList))
                {
                    // show 7 commands in each line
                    foreach (var batch in simpleList.Batch(7))
                    {
                        description += "> " + string.Join(" ", batch.Select(x => x.FullAliases[0])) + "\n\n";
                    }
                }
                
                if (attributePair.Equals(last))
                {
                    embed.WithFooter("Made by Ly#3449, original concept by zappin#1312, version " + Program.Version);
                }
                else
                {
                    // remove last newlines
                    description = description.Substring(0, description.Length - 1);
                }

                embeds.Add(embed.WithDescription(description));
            }
            
            var builder = new LocalMessage();
            var lastEmbed = embeds.Last();
            var addedReaction = false;
            foreach (var embed in embeds)
            {
                try
                {
                    await Context.Author.SendMessageAsync(builder.WithEmbeds(embed));
                    if (!embed.Equals(lastEmbed))
                    {
                        await Task.Delay(500);
                    }

                    if (!addedReaction)
                    {
                        await Context.Message.AddReactionAsync(new LocalEmoji("✅"));
                    }
                }
                catch (Exception) // in case someone has blocked dms
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
        [Description("Invite yours truly to your server")]
        public DiscordCommandResult Invite()
        {
            return Reply(
                Program.GetEmbed(string.Format(
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

        [Command("status")]
        [Description("Display current uptime, server info and more")]
        public DiscordCommandResult Status()
        {
            return Embed(string.Format(
                STATUS_MESSAGE,
                Program.Uptime.ToReadable(),
                Context.Bot.GetGuilds().Count,
                Program.Version
            ));
        }

        [Command("changelog", "changes", "whatsnew")]
        [Description("Displays latest changes to the bot")]
        public async Task<DiscordCommandResult> Changelog()
        {
            // if not fetched yet, do that
            if (_changelog == null)
            {
                var response = await _client.GetAsync(CHANGELOG_URL);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _changelog = "Something went wrong!";
                }
                else
                {
                    var text = await response.Content.ReadAsStringAsync();
                    foreach (var s in Regex.Split(text, "\r\n|\r|\n"))
                    {
                        var segment = s.Split(" ");
                        if (segment.Length > 0 && segment[0].Length > 0)
                        {
                            if (segment[0][0] == '#')
                            {
                                _changelog += "**" + String.Join(" ", segment.Skip(1)) + "**\n";
                                continue;
                            }
                            else if (segment[0][0] == '*')
                            {
                                _changelog += "- " + String.Join(" ", segment.Skip(1)) + "\n";
                                continue;
                            }
                        }

                        _changelog += s + "\n";
                    }
                }
            }

            return EmbedReply(_changelog!);
        }
    }
}