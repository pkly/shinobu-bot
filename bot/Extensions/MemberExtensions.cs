using System.Net.Http;
using System.Threading.Tasks;
using Disqord;
using Disqord.Gateway;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = SixLabors.ImageSharp.Color;

namespace Shinobu.Extensions
{
    public static class MemberExtensions
    {
        private static readonly HttpClient CLIENT = new()
        {
            DefaultRequestHeaders =
            {
                {"User-Agent", Program.UserAgent}
            }
        };
        
        public static string NickOrName(this IMember member)
        {
            return member.Nick != null && member.Nick.Length > 0 ? member.Nick : member.Name;
        }

        public static async Task<Image> Avatar(this IMember member, int size = 256, CdnAssetFormat format = CdnAssetFormat.Png)
        {
            return await Image.LoadAsync(await CLIENT.GetStreamAsync(member.GetAvatarUrl(format, size)));
        }

        public static Color? Color(this IMember member)
        {
            foreach (var role in member.GetRoles())
            {
                var color = role.Value.Color;
                if (color != null)
                {
                    return new Color(new Rgba32(color.Value.R, color.Value.G, color.Value.B));
                }
            }

            return null;
        }
    }
}