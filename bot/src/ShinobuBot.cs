using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Sharding;
using Disqord.Gateway;
using Disqord.Rest;
using Disqord.Sharding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Qmmands;
using Shinobu.TypeParsers;

namespace Shinobu
{
    public class ShinobuBot : DiscordBotSharder
    {
        public ShinobuBot(
            IOptions<DiscordBotSharderConfiguration> options,
            ILogger<DiscordBotSharder> logger,
            IPrefixProvider prefixes,
            ICommandQueue queue,
            CommandService commands,
            IServiceProvider services,
            DiscordClientSharder client) : base(options, logger, prefixes, queue, commands, services, client)
        {}
        
        protected override ValueTask AddTypeParsersAsync(
            CancellationToken cancellationToken = new CancellationToken()
        ) {
            Commands.AddTypeParser(new ReadOnlyCollectionIMemberTypeParser());
            return base.AddTypeParsersAsync(cancellationToken);
        }
    }
}