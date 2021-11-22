using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using CommandLine;
using Disqord;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Shinobu.Database;
using Shinobu.Models.Assets;
using Shinobu.Services;

namespace Shinobu
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal sealed class Program
    {
        private const string REACTION_COMMANDS_FILE = "reaction-commands-api.json";

        private static readonly DateTime BOOT = DateTime.UtcNow;
            
        public static Dictionary<string, ApiCommand> ApiCommands { get; private set; } = new();
        public static string AssetsPath { get; private set; } = "";

        public static TimeSpan Uptime => DateTime.UtcNow - BOOT;
        
        public static long Timestamp => Convert.ToInt64((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds);

        public static string Version
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                if (version == null)
                {
                    return "Unknown";
                }

                return version.ToString(3);
            }
        }

        public static string UserAgent => "ShinobuBot/v" + Version;

        public static Color Color => System.Drawing.ColorTranslator.FromHtml(Env("EMBED_COLOR") ?? "#00ff00");

        public static LocalEmbed GetEmbed(string? description = null)
        {
            var embed = (new LocalEmbed())
                .WithColor(Color);

            if (description != null) {
                embed.WithDescription(description);
            }

            return embed;
        }

        public static string? Env(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
        
        static int Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => {
                var path = o.EnvPath;
                DotNetEnv.Env.LoadMulti(new[] {
                    string.IsNullOrEmpty(path) ? ".env" : path + "/.env",
                    string.IsNullOrEmpty(path) ? ".env.local" : path + "/.env.local"
                });

                AssetsPath = o.AssetsPath ?? "assets/";
                if (!Directory.Exists(AssetsPath))
                {
                    throw new Exception("Assets directory not found");
                }

                ApiCommands = JsonConvert.DeserializeObject<Dictionary<string, ApiCommand>>(File.ReadAllText(AssetsPath + "/" + REACTION_COMMANDS_FILE));
            });
            
            string? token = Env("BOT_TOKEN");
            if (string.IsNullOrEmpty(token)) {
                Console.WriteLine(".env file could not be read, recheck your .env file or specify env-path as a cli argument");
                return -1;
            }

            if (!Enum.TryParse(Env("MIN_LOG_LEVEL"), out LogEventLevel minLevel))
            {
                minLevel = LogEventLevel.Verbose;
            }

            var host = new HostBuilder()
                .ConfigureHostConfiguration(x => x.AddCommandLine(args))
                .ConfigureAppConfiguration(x => {
                    x.AddCommandLine(args);
                })
                .ConfigureServices(x =>
                {
                    x.AddSingleton(y => new HttpClient()
                    {
                        DefaultRequestHeaders =
                        {
                            {"User-Agent", UserAgent}
                        }
                    });
                    x.AddSingleton(typeof(Random));
                    x.AddSingleton(typeof(FontService));
                    x.AddDbContext<ShinobuDbContext>(y => 
                        y.UseMySql(Env("DB_STRING")!, new MariaDbServerVersion(new Version(10, 5, 5)))
                            .EnableSensitiveDataLogging()
                            .EnableDetailedErrors()
                    );
                })
                .ConfigureLogging(x => {
                    var logger = new LoggerConfiguration()
                        .MinimumLevel.Is(minLevel)
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
                        .WriteTo.File($"logs/log-{DateTime.Now:HH_mm_ss}.txt", LogEventLevel.Verbose, fileSizeLimitBytes: null, buffered: true)
                        .CreateLogger();
                    x.AddSerilog(logger, true);

                    x.Services.Remove(x.Services.First(x => x.ServiceType == typeof(ILogger<>)));
                    x.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                })
                .ConfigureDiscordBotSharder<ShinobuBot>((context, bot) => {
                    bot.Token = token;
                    bot.UseMentionPrefix = true;
                    bot.Prefixes = new[] { Env("PREFIX") };
                    bot.Intents = GatewayIntents.RecommendedUnprivileged;

                    if (null != Env("OWNER_IDS"))
                    {
                        List<Snowflake> ids = new();
                        foreach (var id in Env("OWNER_IDS")!.Split(","))
                        {
                            try
                            {
                                var ownerId = Convert.ToUInt64(id);
                                ids.Add(ownerId);
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }

                        bot.OwnerIds = ids;
                    }
                    
                    bot.Activities = new[]
                    {
                        new LocalActivity(Env("PREFIX") + "help | v" + Version, ActivityType.Playing),
                    };
                })
                .UseDefaultServiceProvider(x => x.ValidateOnBuild = true)
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<ShinobuDbContext>().Database.Migrate();
            }
            
            host.Run();
            return 0;
        }
    }
}
