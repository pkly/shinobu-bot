using System;
using System.Text.Json;
using Disqord;
using CommandLine;

namespace Shinobu
{
    class Helper
    {
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
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => {
                var path = o.EnvPath == null ? null : o.EnvPath;
                DotNetEnv.Env.LoadMulti(new[] {
                    path + "/.env",
                    path + "/.env.local"
                });
            });
        }
    }
}