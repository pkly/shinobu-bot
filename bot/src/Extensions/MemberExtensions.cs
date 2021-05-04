using System.Net.Http;
using System.Threading.Tasks;
using Disqord;
using SixLabors.ImageSharp;

namespace Shinobu.Extensions
{
    public static class MemberExtensions
    {
        private static readonly HttpClient _client = new HttpClient();
        
        public static string NickOrName(this IMember member)
        {
            return member.Nick != null && member.Nick.Length > 0 ? member.Nick : member.Name;
        }

        public static async Task<Image> Avatar(this IMember member, int size = 256, ImageFormat format = ImageFormat.Png)
        {
            return await Image.LoadAsync(await _client.GetStreamAsync(member.GetAvatarUrl(format, size)));
        }
    }
}