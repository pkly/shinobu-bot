using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using Shinobu.Models.Assets;

namespace Shinobu
{
    static class Helper
    {
        private const string REACTION_COMMANDS_FILE = "reaction-commands-api.json";
        
        public static Dictionary<string, ApiCommand> ApiCommands { get; private set; }
        public static string AssetsPath { get; private set; }
        
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

                AssetsPath = o.AssetsPath ?? "assets/";
                if (!Directory.Exists(AssetsPath))
                {
                    throw new Exception("Assets directory not found");
                }

                ApiCommands = JsonConvert.DeserializeObject<Dictionary<string, ApiCommand>>(File.ReadAllText(AssetsPath + "/" + REACTION_COMMANDS_FILE));
            });
        }
    }
}