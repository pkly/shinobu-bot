using CommandLine;

namespace Shinobu 
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class Options
    {
        [Option('e', "env-path", Required = false, HelpText = "Path from which to load the .env files")]
        public string? EnvPath { get; set; } = null;

        [Option('a', "assets-path", Required = false, HelpText = "Assets path")]
        public string? AssetsPath { get; set; } = null;
    }
}