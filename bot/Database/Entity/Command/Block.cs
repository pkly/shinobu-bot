using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shinobu.Database.Entity.Command
{
    [Table("c__block")]
    public class Block
    {
        [Key]
        public ulong Id { get; set; }
        
        public ulong Requester { get; set; }
        public ulong Blocked { get; set; }
    }
}