using System;

namespace PokeBot.Dtos
{
    public class PokeBattleForReturnDto
    {
        public Guid BattleTokenId { get; set; }
        public ulong UserOneId { get; set; }
        public ulong UserTwoId { get; set; }
    }
}