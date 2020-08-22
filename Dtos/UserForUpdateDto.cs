using System;

namespace PokeBot.Dtos
{
    public class UserForUpdateDto
    {
        public Guid BattleTokenId { get; set; }
        public UserForUpdateDto(Guid battleTokenId)
        {
            BattleTokenId = battleTokenId;
        }
    }
}