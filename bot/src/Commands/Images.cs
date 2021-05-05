using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Qmmands;
using Shinobu.Attributes;
using Shinobu.Extensions;
using Shinobu.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Shinobu.Commands
{
    [Section("Image generation")]
    public class Images : ShinobuModuleBase
    {
        private const string LOVE_STRING = "{0} and {1}'s love is at **{2}%**\n\nYour shipname is **{3}**";

        private readonly Image<Rgba32> _bonk;
        private readonly Image<Rgba32> _ejected;
        private readonly Image<Rgba32> _marry;
        private readonly Image<Rgba32> _milk;
        private readonly Image<Rgba32> _pin;
        private readonly Image<Rgba32> _sauce;
        private readonly Image<Rgba32> _tuck;
        private readonly Image<Rgba32> _love;
        
        private readonly RangeHelper<string> _loveRanges = new RangeHelper<string>(new Range<string>[]
        {
            new Range<string>(0, "You guys aren't even remotely a match...", 0),
            new Range<string>(1, "Maybe you're better off as distant friends", 25),
            new Range<string>(26, "You guys are friends but I sense no romance", 50),
            new Range<string>(51, "There's a connection between the two!", 75),
            new Range<string>(76, "Love is in the air <3", 99),
            new Range<string>(100, "Such a cute couple <3 <3 <3")
        });

        public Images()
        {
            _bonk = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/bonk.jpg");
            _ejected = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/ejected.png");
            _marry = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/marry.jpg");
            _milk = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/milk.jpg");
            _pin = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/pin.jpg");
            _sauce = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/sauce.jpg");
            _tuck = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/tuck.jpg");
            _love = Image.Load<Rgba32>(Helper.AssetsPath + "/images/meme/love.png");
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
        
        [Command("love", "lovecalc", "ship", "calclove")]
        public async Task<DiscordCommandResult> Love(IMember memberA, IMember? memberB = null)
        {
            if (memberB == null)
            {
                memberB = memberA;
                memberA = (IMember) Context.Author;
            }

            var name = memberA.NickOrName().Substring(0, (int) Math.Ceiling((double) memberA.NickOrName().Length / 2)) +
                       memberB.NickOrName().Substring(0, (int) Math.Ceiling((double) memberB.NickOrName().Length / 2));

            var random = new Random((int) memberA.Id.RawValue + (int) memberB.Id.RawValue);
            var result = Math.Max(
                Math.Min(
                    random.Next(100),
                    100
                ),
                0
            );
            
            var stream = new MemoryStream();
            using (Image empty = new Image<Rgba32>(625, 250))
            using (Image copy = _love.Clone())
            {
                using (Image firstAvatar = await memberA.Avatar())
                using (Image secondAvatar = await memberB.Avatar())
                {
                    firstAvatar.Mutate(x => x.Resize(250, 250));
                    secondAvatar.Mutate(x => x.Resize(250, 250));
                
                    empty.Mutate(x => 
                        x.DrawImage(firstAvatar, new Point(0, 0), 1)
                            .DrawImage(secondAvatar, new Point(375, 0), 1)
                    );
                    empty.Mutate(x => x.DrawImage(copy, new Point(0, 0), 1));
                }

                await empty.SaveAsPngAsync(stream);
            }

            return RespondWithAttachment(
            GetEmbed(string.Format(
                    LOVE_STRING,
                    memberA.Mention,
                    memberB.Mention,
                    result,
                    name
                ))
                .WithFooter(_loveRanges.GetValue(result)), 
                stream
            );
        }
        
        //
        // [Command("sus", "amongus", "amogus", "eject", "imposter", "impostor")]
        // public async Task<DiscordCommandResult> Imposter(IMember? member = null)
        // {
        //     
        // }
    }
}