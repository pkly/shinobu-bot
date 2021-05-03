using System.Collections.Generic;

namespace Shinobu.Models.Assets
{
    public class ApiCommand
    {
        // Api-specific
        public string Url;
        public string? ReplaceUrl = null;
        public string Path = "url"; // default api path for the image

        // Identifiers
        public string Name;
        public IList<string> Aliases;
        
        // Texts
        public IList<string> Mention;
        public IList<string> NoMention;

        // Help parts
        public string Group = "Unknown";
        public int? SimpleGroup = null;
    }
}