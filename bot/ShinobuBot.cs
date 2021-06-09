using System;
using System.Threading.Tasks;
using System.Threading;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Sharding;
using Disqord.Sharding;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Shinobu.Attributes;
using Shinobu.Commands;
using Shinobu.TypeParsers;

namespace Shinobu
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShinobuBot : DiscordBotSharder
    {
        public ShinobuBot(
            IOptions<DiscordBotSharderConfiguration> options,
            // ReSharper disable once ContextualLoggerProblem
            ILogger<DiscordBotSharder> logger,
            IServiceProvider services,
            DiscordClientSharder client) : base(options, logger, services, client)
        {}
        
        protected override ValueTask AddTypeParsersAsync(
            CancellationToken cancellationToken = new()
        )
        {
            var value = base.AddTypeParsersAsync(cancellationToken);
            // add custom ones last
            Commands.AddTypeParser(new ErrorIgnoringMemberTypeParser());
            return value;
        }

        protected override ValueTask AddModulesAsync(
            CancellationToken cancellationToken = new()
        )
        {
            // register the api commands
            foreach (var pair in Program.ApiCommands)
            {
                Commands.AddModule(
                    x => x.AddCommand(context => Dynamic.DoReaction((DiscordCommandContext) context),
                    x =>
                    {
                        x.WithName(pair.Key);
                        x.AddAlias(pair.Key);
                        x.AddAttribute(new SectionAttribute("Reactions", "You can tag users after the command (one or more!)"));
                        x.AddAttribute(new SimpleCommandAttribute());

                        foreach (var alias in pair.Value.Aliases)
                        {
                            x.AddAlias(alias);
                        }

                        // just pass everything
                        x.AddParameter(
                            typeof(IMember),
                            x => 
                                x.WithIsMultiple(true)
                                    .WithIsOptional(true)
                                    .WithCustomTypeParserType(typeof(ErrorIgnoringMemberTypeParser))
                                    .WithDefaultValue(new IMember[] {})
                                    .WithName("members")
                        );
                    })
                );
            }
            return base.AddModulesAsync(cancellationToken);
        }
    }
}