using System;

namespace PokeBot.Dtos
{
    public class PokeBattleForCreationDto
    {
        public ulong UserOneId { get; set; }
        public ulong UserTwoId { get; set; }
        public Guid BattleTokenId { get; set; }

        public PokeBattleForCreationDto(Guid battleId, ulong userOneId, ulong userTwoId)
        {
            BattleTokenId = battleId;
            UserOneId = userOneId;
            UserTwoId = userTwoId;
        }
    }
}