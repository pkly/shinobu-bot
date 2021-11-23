using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot.Hosting;
using Disqord.Rest;
using Newtonsoft.Json;
using Shinobu.Extensions;
using Shinobu.Models.Assets.Event.Winter;
using Shinobu.Services.Profile;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;

namespace Shinobu.Services.Event
{
    public class WinterService : DiscordBotService
    {
        private readonly Random _random;
        private readonly FontService _fontService;
        private readonly WalletService _walletService;

        private readonly Image<Rgba32> _bg;
        private readonly List<EventQuery> _eventQueries;

        private EventQuery? _currentQuery;
        private int _points = 0;
        private readonly object _lock = new ();

        private readonly List<double> _claimMultipliers = new ()
        {
            1,
            0.4,
            0.15
        };

        private Snowflake? _channel = null;
        private short _answerCount = 0;
        private List<Snowflake> _userAnswerIds = new ();
        private DateTime? _firstAnswerTime = null;

        private readonly Font _eventFont;

        public WinterService(Random random, FontService fontService, WalletService walletService)
        {
            _random = random;
            _fontService = fontService;
            _walletService = walletService;
            _bg = Image.Load<Rgba32>(Program.AssetsPath + "/images/event/winter.png");
            _eventQueries = JsonConvert.DeserializeObject<List<EventQuery>>(File.ReadAllText(Program.AssetsPath + "/event/winter/words.json")) ?? new List<EventQuery>();
            _eventFont = _fontService.FontFamily.CreateFont(120, FontStyle.Bold);

            ulong temp;
            if (ulong.TryParse(Program.Env("WINTER_EVENT_CHANNEL_ID"), out temp))
            {
                _channel = new Snowflake(temp);
            }
        }

        public async Task RandomizeTask()
        {
            if (null == _channel)
            {
                return;
            }
            
            lock (_lock)
            {
                _currentQuery = _eventQueries.Random();
                _answerCount = 0;
                _points = _random.Next(100, 1000) * (_random.Next(5) == 4 ? 5 : 1);
                _userAnswerIds = new List<Snowflake>();
                _firstAnswerTime = null;
            }

            if (null == _currentQuery)
            {
                return;
            }

            var stream = new MemoryStream();
            using (Image copy = _bg.Clone())
            {
                copy.Mutate(x => 
                    x.DrawTextCentered(_currentQuery.Query, _eventFont, Color.White, 205)
                );
                
                await copy.SaveAsPngAsync(stream);
            }
            
            stream.Rewind();

            var message = new LocalMessage()
                .WithEmbeds(Program.GetEmbed(string.Format(
                        "🔔**A Christmas spirit has visited!**🔔\n\n> Type the word/do the math to claim **`{0}`** points!",
                        _points
                    ))
                .WithImageUrl("attachment://file.png"))
                .AddAttachment(new LocalAttachment(stream, "file.png"));

            await Client.SendMessageAsync((Snowflake) _channel, message);
        }
        
        protected override async ValueTask OnNonCommandReceived(BotMessageReceivedEventArgs e)
        {
            if (null == _channel || null == _currentQuery || e.Message.Id == Client.CurrentUser.Id || _channel != e.ChannelId)
            {
                return;
            }

            if (_answerCount < _claimMultipliers.Count && 
                _currentQuery.IsValidAnswer(e.Message.Content) && 
                !_userAnswerIds.Contains(e.Member.Id) && 
                (_firstAnswerTime == null || _firstAnswerTime.Value.AddSeconds(5) > DateTime.UtcNow))
            {
                var message = new LocalMessage()
                    .WithReply(e.Message.Id);

                int reward;
                lock (_lock)
                {
                    if (null == _firstAnswerTime)
                    {
                        _firstAnswerTime = DateTime.UtcNow;
                    }
                    
                    _userAnswerIds.Add(e.Member.Id);
                    reward = (int) Math.Round(_points * _claimMultipliers[_answerCount++]);

                    message.WithEmbeds(Program.GetEmbed(string.Format(
                        "> Good job <@{0}>, you get **{1}** points!",
                        e.Member.Id,
                        reward
                    )));
                }

                await Client.SendMessageAsync(e.ChannelId, message);
                await _walletService.AddPoints(e.AuthorId, (ulong) reward);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Client.WaitUntilReadyAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(_random.Next(6, 13)), stoppingToken);
                if (stoppingToken.IsCancellationRequested)
                {
                    break; // graceful exit
                }

                await RandomizeTask();
            }
        }
    }
}