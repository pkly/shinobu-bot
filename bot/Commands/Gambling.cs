using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Microsoft.EntityFrameworkCore;
using Qmmands;
using Shinobu.Attributes;
using Shinobu.Database;
using Shinobu.Extensions;
using Shinobu.Services.Profile;

namespace Shinobu.Commands
{
    [Section("Gambling")]
    public class Gambling : ShinobuModuleBase
    {
        private readonly WalletService _walletService;
        private readonly Random _random;
        private readonly ShinobuDbContext _context;
        private const int BET_MAX = 666;

        public Gambling(WalletService walletService, Random random, ShinobuDbContext context)
        {
            _walletService = walletService;
            _random = random;
            _context = context;
        }

        [Command("leaderboard", "lb")]
        public async Task<DiscordResponseCommandResult> Leaderboard()
        {
            var results = await _context.Wallets.OrderByDescending(x => x.Points)
                .Take(20)
                .ToListAsync();

            if (0 == results.Count)
            {
                return EmbedReply("No one on the leaderboard yet! Join the fun!");
            }

            var users = new List<string>();

            int counter = 1;
            foreach (var wallet in results)
            {
                users.Add(counter + ". - " + Mention.User(new Snowflake(wallet.UserId)) + " - " + wallet.Points);
                counter++;
            }

            return EmbedReply(string.Join("\n", users));
        }

        [Command("balance", "bal")]
        public async Task<DiscordResponseCommandResult> Balance()
        {
            var wallet = await _walletService.GetWallet(Context.Author.Id);

            return EmbedReply(string.Format(
                "You have {0} points in your wallet",
                wallet.Points
            ));
        }
        
        [Command("bet")]
        public async Task<DiscordResponseCommandResult> Bet([Minimum(5)][Maximum(BET_MAX)]uint amount)
        {
            var wallet = await _walletService.GetWallet(Context.Author.Id);
            if (wallet.Points < amount)
            {
                return EmbedReply(string.Format(
                    "You only have {0} points in your wallet",
                    wallet.Points
                ));
            }

            return await DoBet(wallet.UserId, 43, amount);
        }

        // needs a rework, too lazy to do it now since the event is over
        // [Command("bet-all")]
        // public async Task<DiscordCommandResult> BetAll()
        // {
        //     var wallet = await _walletService.GetWallet(Context.Author.Id);
        //     if (wallet.Points <= 5)
        //     {
        //         return EmbedReply(string.Format(
        //             "You only have {0} points in your wallet, minimum is 5",
        //             wallet.Points
        //         ));
        //     }
        //     
        //     return View(new ConfirmView("Are you absolutely sure you want your big chance at riches?", DoBetAll));
        // }

        private async Task<DiscordResponseCommandResult> DoBetAll()
        {
            return await DoBet(Context.Author.Id, 5, 0, 3);
        }

        private async Task<DiscordResponseCommandResult> DoBet(ulong userId, uint winChance, ulong amount = 0, uint multiplier = 1)
        {
            var wallet = await _walletService.GetWallet(userId);
            if (0 == amount)
            {
                amount = wallet.Points;
            }
            
            if (_random.FitsPercentage((short) winChance))
            {
                var lowest = await _walletService.GetLowestWallet();
                await _walletService.SetPoints(Context.Author.Id, wallet.Points - amount);

                if (null == lowest || wallet.Id == lowest.Id)
                {
                    return EmbedReply(string.Format(
                        "You lose **{0}** points! They're off to somewhere!",
                        amount
                    ));
                }

                await _walletService.AddPoints(lowest.UserId, amount / 2);
                return EmbedReply(string.Format(
                    "You lose **{0}** points! {1} takes half of them away!",
                    amount,
                    "<@" + lowest.UserId + ">"
                ));
            }

            ulong reward = amount * multiplier;
            await _walletService.AddPoints(wallet.UserId, reward);
            return EmbedReply(string.Format(
                "You win **{0}** points!",
                reward
            ));
        }
    }
}