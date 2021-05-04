using System;
using System.IO;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Qmmands;
using Shinobu.Attributes;
using Shinobu.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Shinobu.Commands
{
    [Section("Image generation")]
    public class Images : ShinobuModuleBase
    {
        private readonly Image<Rgba32> _bonk;
        private readonly Image<Rgba32> _ejected;
        private readonly Image<Rgba32> _marry;
        private readonly Image<Rgba32> _milk;
        private readonly Image<Rgba32> _pin;
        private readonly Image<Rgba32> _sauce;
        private readonly Image<Rgba32> _tuck;

        public Images()
        {
            _bonk = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/bonk.jpg");
            _ejected = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/ejected.png");
            _marry = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/marry.jpg");
            _milk = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/milk.jpg");
            _pin = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/pin.jpg");
            _sauce = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/sauce.jpg");
            _tuck = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/tuck.jpg");
        }

        [Command("milk")]
        public async Task<DiscordCommandResult> Milk(IMember member)
        {
            var stream = new MemoryStream();
            using (Image copy = _milk.Clone())
            {
                using (Image authorAvatar = await ((IMember) Context.Author).Avatar())
                using (Image memberAvatar = await member.Avatar())
                {
                    authorAvatar.Mutate(x => x.Resize(190, 190));
                    memberAvatar.Mutate(x => x.Resize(90, 90));
                
                    copy.Mutate(x => 
                        x.DrawImage(authorAvatar, new Point(110, 35), 1)
                            .DrawImage(memberAvatar, new Point(420, 110), 1)
                    );
                }

                await copy.SaveAsPngAsync(stream);
            }

            return RespondWithAttachment($"{Context.Author.Mention} has milked {member.Mention}", stream);
        }

        [Command("bonk")]
        public async Task<DiscordCommandResult> Bonk(IMember member)
        {
            var stream = new MemoryStream();
            using (Image copy = _bonk.Clone())
            {
                using (Image authorAvatar = await ((IMember) Context.Author).Avatar())
                using (Image memberAvatar = await member.Avatar())
                {
                    authorAvatar.Mutate(x => x.Resize(180, 180));
                    memberAvatar.Mutate(x => x.Resize(140, 140).Rotate(45));
                    
                    copy.Mutate(x => 
                        x.DrawImage(authorAvatar, new Point(145, 90), 1)
                            .DrawImage(memberAvatar, new Point(420, 230), 1)
                    );
                }

                await copy.SaveAsPngAsync(stream);
            }

            return RespondWithAttachment($"{Context.Author.Mention} has bonked {member.Mention}", stream);
        }

        [Command("sauce")]
        public async Task<DiscordCommandResult> Sauce(IMember member)
        {
            var stream = new MemoryStream();
            using (Image copy = _sauce.Clone())
            {
                using (Image memberAvatar = await member.Avatar())
                {
                    memberAvatar.Mutate(x => x.Resize(260, 260));
                    
                    copy.Mutate(x => x.DrawImage(memberAvatar, new Point(190, 282), 0.7f));
                }

                await copy.SaveAsPngAsync(stream);
            }

            return RespondWithAttachment($"Fresh tub of {member.Mention} sauce", stream);
        }

        [Command("marry")]
        public async Task<DiscordCommandResult> Marry(IMember member)
        {
            var stream = new MemoryStream();
            using (Image copy = _marry.Clone())
            {
                using (Image authorAvatar = await ((IMember) Context.Author).Avatar())
                using (Image memberAvatar = await member.Avatar())
                {
                    authorAvatar.Mutate(x => x.Resize(150, 150));
                    memberAvatar.Mutate(x => x.Resize(150, 150));
                    
                    copy.Mutate(x =>
                        x.DrawImage(authorAvatar, new Point(400, 35), 1)
                            .DrawImage(memberAvatar, new Point(160, 35), 1)
                    );
                }

                await copy.SaveAsPngAsync(stream);
            }

            return RespondWithAttachment($"{Context.Author.Mention} has married {member.Mention}", stream);
        }
        //
        // [Command("sus", "amongus", "amogus", "eject", "imposter", "impostor")]
        // public async Task<DiscordCommandResult> Imposter(IMember? member = null)
        // {
        //     
        // }
    }
}