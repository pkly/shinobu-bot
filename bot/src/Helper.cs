using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using Shinobu.Models.Assets;

namespace Shinobu
{
    class Helper
    {
        private const string REACTION_COMMANDS_FILE = "reaction-commands-api.json";
        
        public static Dictionary<string, ApiCommand> ApiCommands { get; private set; }
        
        public static long GetTimestamp()
        {
            return Convert.ToInt64((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        public static string? Env(string name)
        {
            return System.Environment.GetEnvironmentVariable(name);
        }

        public static void Init(string[] args)
        {
            // this should prolly be moved
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => {
                var path = o.EnvPath;
                DotNetEnv.Env.LoadMulti(new[] {
                    path + "/.env",
                    path + "/.env.local"
                });

                var assetsPath = o.AssetsPath ?? "assets/";
                if (!Directory.Exists(assetsPath))
                {
                    throw new Exception("Assets directory not found");
                }

                ApiCommands = JsonConvert.DeserializeObject<Dictionary<string, ApiCommand>>(File.ReadAllText(assetsPath + "/" + REACTION_COMMANDS_FILE));
            });
        }
    }
}