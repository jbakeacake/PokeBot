using System;

namespace PokeBot.Models
{
    public class PokeBattleData
    {
        public int Id { get; set; }
        public Guid BattleTokenId { get; set; }
        public int UserOneId { get; set; }
        public int UserTwoId { get; set; }
        public int UserOnePokemonId { get; set; }
        public int UserTwoPokemonId { get; set; }
        
    }
}