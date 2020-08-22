using System;

namespace PokeBot.Models
{
    public class PokeBattleData
    {
        public int Id { get; set; }
        public Guid BattleTokenId { get; set; }
        public ulong UserOneId { get; set; }
        public ulong UserTwoId { get; set; }
    }
}