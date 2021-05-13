using System.Collections.Generic;
using Shinobu.Utility;

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
        public IList<string> Aliases = new List<string>();
        
        // Texts
        public IList<string> Mention = new List<string>();
        public IList<string> NoMention = new List<string>();
        
        // // Multi-actions
        public RangeHelper<IList<string>> Actions = new RangeHelper<IList<string>>();

        // Help parts
        public string Group = "Unknown";
        public int? SimpleGroup = null;

        public string GetFixedImageUrl(string imageUrl)
        {
            if (ReplaceUrl == null)
            {
                return imageUrl;
            }

            return string.Format(ReplaceUrl, imageUrl);
        }
    }
}