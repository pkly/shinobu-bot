using CommandLine;

namespace Shinobu 
{
    class Options
    {
        [Option('e', "env-path", Required=false, HelpText="Path from which to load the .env files")]
        public string? EnvPath { get; set; }
        
        [Option('a', "assets-path", Required=false, HelpText="Assets path")]
        public string? AssetsPath { get; set; }
    }
}