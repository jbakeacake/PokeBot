using System.Collections.Generic;

namespace PokeBot.Models
{
    public class User
    {
        public int Id { get; set; }
        public ulong DiscordId { get; set; }
        public ICollection<Pokemon> PokeCollection { get; set; }
    }
}