using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shinobu.Database.Entity.Profile
{
    [Table("p__wallet")]
    public class Wallet
    {
        [Key]
        public ulong Id { get; set; }
        
        public ulong UserId { get; set; }
        
        public ulong Points { get; set; }
    }
}