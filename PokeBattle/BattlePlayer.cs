using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokeBot.Models;
using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle
{
    public class BattlePlayer
    {
        public PokeEntity CurrentPokemon { get; set; }
        // public Dictionary<string, PokeEntity> PokeInventory { get; set; }
        public Guid BattleTokenID { get; set; }
        public int UserId { get; set; }
        public int PokemonRecordId { get; set; } // Record Id, not literal PokeId

        public BattlePlayer(int userId, int pokemonRecordId, Guid battleTokenId)
        {
            UserId = userId;
            PokemonRecordId = pokemonRecordId;
            BattleTokenID = battleTokenId;
        }

        public void InitializeCurrentPokemon(Pokemon pokemon)
        {
            CurrentPokemon = new PokeEntity(pokemon);
        }
    }
}