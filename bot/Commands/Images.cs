using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Newtonsoft.Json;
using Qmmands;
using Shinobu.Attributes;
using Shinobu.Extensions;
using Shinobu.Models.Api.OpenWeather;
using Shinobu.Utility;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;

namespace Shinobu.Commands
{
    [Section("Image generation")]
    public class Images : ShinobuModuleBase
    {
        private const string LOVE_STRING = "{0} and {1}'s love is at **{2}%**\n\nYour shipname is **{3}**";
        private const string WEATHER_URL = "http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&appid={1}";
        private const string FLAG_URL = "https://www.countryflags.io/{0}/flat/64.png";
        private const string ICON_URL = "http://openweathermap.org/img/wn/{0}@2x.png";
        
        private readonly FontCollection _fontCollection = new FontCollection();
        private readonly FontFamily _fontFamily;
        private readonly Font _bold;

        private readonly Color _weatherLightBgColor = new Color(new Rgba32(255, 233, 89));
        private readonly Color _weatherDarkBgColor = new Color(new Rgba32(67, 68, 92));
        private readonly Font _weatherTitleFont;
        private readonly Font _weatherFont;
        private readonly Font _weatherMainFont;
        private readonly Font _weatherHumidityFont;
        private readonly Font _weatherSpeedFont;

        private readonly Image<Rgba32> _bonk;
        private readonly Image<Rgba32> _ejected;
        private readonly Image<Rgba32> _marry;
        private readonly Image<Rgba32> _milk;
        private readonly Image<Rgba32> _pin;
        private readonly Image<Rgba32> _sauce;
        private readonly Image<Rgba32> _tuck;
        private readonly Image<Rgba32> _love;

        private readonly Image<Rgba32> _weatherHumid;
        private readonly Image<Rgba32> _weatherWind;

        private readonly Random _random;
        private readonly HttpClient _client;
        
        private readonly RangeHelper<string> _loveRanges = new RangeHelper<string>(new Range<string>[]
        {
            new Range<string>(0, "You guys aren't even remotely a match...", 0),
            new Range<string>(1, "Maybe you're better off as distant friends", 25),
            new Range<string>(26, "You guys are friends but I sense no romance", 50),
            new Range<string>(51, "There's a connection between the two!", 75),
            new Range<string>(76, "Love is in the air <3", 99),
            new Range<string>(100, "Such a cute couple <3 <3 <3")
        });

        private readonly Dictionary<bool, string> _imposterDictionary = new Dictionary<bool, string>()
        {
            {false, "{0} was not The Imposter"},
            {true, "{0} was The Imposter"}
        };

        public Images(Random random, HttpClient client)
        {
            _bonk = Image.Load<Rgba32>(Program.AssetsPath + "/images/meme/bonk.jpg");
            _ejected = Image.Load<Rgba32>(Program.AssetsPath + "/images/meme/ejected.png");
            _marry = Image.Load<Rgba32>(Program.AssetsPath + "/images/meme/marry.jpg");
            _milk = Image.Load<Rgba32>(Program.AssetsPath + "/images/meme/milk.jpg");
            _pin = Image.Load<Rgba32>(Program.AssetsPath + "/images/meme/pin.png");
            _sauce = Image.Load<Rgba32>(Program.AssetsPath + "/images/meme/sauce.jpg");
            _tuck = Image.Load<Rgba32>(Program.AssetsPath + "/images/meme/tuck.jpg");
            _love = Image.Load<Rgba32>(Program.AssetsPath + "/images/meme/love.png");

            _weatherHumid = Image.Load<Rgba32>(Program.AssetsPath + "/images/weather/humidity.png");
            _weatherWind = Image.Load<Rgba32>(Program.AssetsPath + "/images/weather/wind.png");

            _fontFamily = _fontCollection.Install(Program.AssetsPath + "/fonts/Roboto.ttf");
            _bold = _fontFamily.CreateFont(34, FontStyle.Bold);
            
            // pre-modify weather and wind images anyway
            _weatherHumid.Mutate(x => x.Resize(31, 31));
            _weatherWind.Mutate(x => x.Resize(35, 35));
            
            // pre-create all the fonts we'll need anyway
            _weatherTitleFont = _fontFamily.CreateFont(80);
            _weatherFont = _fontFamily.CreateFont(40);
            _weatherMainFont = _fontFamily.CreateFont(24);
            _weatherHumidityFont = _fontFamily.CreateFont(21);
            _weatherSpeedFont = _fontFamily.CreateFont(18);

            _random = random;
            _client = client;
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

        [Command("pin")]
        public async Task<DiscordCommandResult> Ping(IMember member)
        {
            var stream = new MemoryStream();
            using (Image copy = _pin.Clone())
            {
                using (Image authorAvatar = await ((IMember) Context.Author).Avatar())
                using (Image memberAvatar = await member.Avatar())
                {
                    authorAvatar.Mutate(x => x.Resize(150, 150));
                    memberAvatar.Mutate(x => x.Resize(150, 150));
                    
                    
                    copy.Mutate(x => 
                        x.DrawImage(authorAvatar, new Point(235, 40), 1)
                            .DrawImage(memberAvatar, new Point(125, 365), 1)
                    );
                }

                await copy.SaveAsPngAsync(stream);
            }

            return RespondWithAttachment($"{Context.Author.Mention} has pinned {member.Mention} down", stream);
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
        
        
        [Command("sus", "amongus", "amogus", "eject", "imposter", "impostor")]
        public async Task<DiscordCommandResult> Imposter(IMember? member = null)
        {
            member ??= (IMember) Context.Author;

            bool result = _random.NextBoolean();
            var text = _imposterDictionary[result];

            var stream = new MemoryStream();
            
            using (Image empty = new Image<Rgba32>(1116, 628))
            using (Image copy = _ejected.Clone())
            {
                var color = member.Color() ?? Color.White;
                if (color.ToHex() == Color.Black.ToHex()) // we don't want to show a black bg with black fill lmao
                {
                    color = Color.White;
                }

                empty.Mutate(x => 
                    x.Fill(color)
                        .DrawImage(copy, 1)
                        .DrawTextCentered(string.Format(text, member.NickOrName()), _bold, Color.White, 314)
                );

                await empty.SaveAsPngAsync(stream);
            }

            return RespondWithAttachment($"{string.Format(text, member.Mention)} {Program.Env(result ? "EMOTE_MINUS" : "EMOTE_PLUS")}", stream);
        }

        [Section("Fun/memes")]
        [Command("weather")]
        public async Task<DiscordCommandResult> Weather([Remainder][Minimum(2)] string query)
        {
            Color textColor = Color.Black;
            Color bgColor = _weatherLightBgColor;

            var response = await _client.GetAsync(string.Format(WEATHER_URL,
                WebUtility.UrlEncode(query),
                Program.Env("OPENWEATHER_API_KEY")
            ));
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return EmbedReply("Please use a valid location name");
            }

            var weather = JsonConvert.DeserializeObject<WeatherResponse>(await response.Content.ReadAsStringAsync());

            if (weather.Dt > weather.Sys.Sunset || weather.Dt < weather.Sys.Sunrise)
            {
                textColor = Color.White;
                bgColor = _weatherDarkBgColor;
            }
            
            var stream = new MemoryStream();
            using (Image flag = await Image.LoadAsync(await _client.GetStreamAsync(string.Format(FLAG_URL, weather.Sys.Country))))
            using (Image icon = await Image.LoadAsync(await _client.GetStreamAsync(string.Format(ICON_URL, weather.Weather[0].Icon))))
            using (Image bg = new Image<Rgba32>(400, 168))
            using (Image empty = new Image<Rgba32>(400, 400))
            {
                flag.Mutate(x => x.Resize(64, 64));
                icon.Mutate(x => x.Resize(160, 160));
                
                bg.Mutate(x => x.Fill(textColor));
                
                empty.Mutate(x => 
                    x.Fill(bgColor)
                        .DrawImage(bg, new Point(0, 20), 0.2f)
                        .DrawTextCentered($"{Convert.ToInt16(weather.Main.Temp).ToString()}°C", _weatherTitleFont, textColor, 220)
                        .DrawTextCentered(weather.Weather[0].Main.ToLower(), _weatherFont, textColor, 345)
                        .DrawTextCentered(weather.Name, _weatherMainFont, textColor, 140)
                        .DrawText(
                            new TextGraphicsOptions(new GraphicsOptions() { BlendPercentage = 0.7f }, new TextOptions() {HorizontalAlignment = HorizontalAlignment.Center}),
                            $"feels like {Convert.ToInt16(weather.Main.Feels_like).ToString()}°C",
                            _weatherMainFont,
                            Brushes.Solid(textColor), 
                            null,
                            new PointF(x.GetCurrentSize().Width / 2, 297)
                        )
                        .DrawImage(_weatherHumid, new Point(35, 250), 1)
                        .DrawTextCentered($"{weather.Main.Humidity}%", _weatherHumidityFont, textColor, new PointF(51, 290))
                        .DrawImage(_weatherWind, new Point(330, 250), 1)
                        .DrawTextCentered($"{Convert.ToInt16(weather.Wind.Speed).ToString()}km/h", _weatherSpeedFont, textColor, new PointF(350, 290))
                        .DrawImage(flag, new Point(-2, 9), 1)
                        .DrawImage(icon, new Point(120, -2), 1)
                );

                await empty.SaveAsPngAsync(stream);
            }

            return RespondWithAttachment(stream);
        }
    }
}