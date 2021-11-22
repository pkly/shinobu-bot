using System;
using System.Linq;
using System.Threading.Tasks;
using Disqord.Bot.Hosting;
using Microsoft.EntityFrameworkCore;
using Shinobu.Database;
using Shinobu.Database.Entity.Profile;

namespace Shinobu.Services.Profile
{
    public class WalletService : DiscordBotService
    {
        private readonly ShinobuDbContext _context;
        
        public WalletService(ShinobuDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet> AddPoints(ulong userId, ulong points)
        {
            var wallet = await GetWallet(userId);
            wallet.Points += points;

            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet> SetPoints(ulong userId, ulong points = 0)
        {
            var wallet = await GetWallet(userId);
            wallet.Points = points;

            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet?> GetLowestWallet()
        {
            try
            {
                return await _context.Wallets.OrderBy(x => x.Points)
                    .FirstAsync();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<Wallet> GetWallet(ulong userId)
        {
            try
            {
                return await _context.Wallets.Where(x => x.UserId == userId)
                    .FirstAsync(); // just load
            }
            catch (InvalidOperationException) // not found
            {
                var wallet = new Wallet()
                {
                    UserId = userId,
                    Points = 0
                };

                await _context.Wallets.AddAsync(wallet);
                return wallet;
            }
        }
    }
}