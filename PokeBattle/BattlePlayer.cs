using System;
using System.Threading.Tasks;
using PokeBot.Dtos;
using PokeBot.Models;
using PokeBot.PokeBattle.Entities;
using PokeBot.PokeBattle.Moves;

namespace PokeBot.PokeBattle
{
    public class BattlePlayer
    {
        public PokeEntity CurrentPokemon { get; set; }
        // public Dictionary<string, PokeEntity> PokeInventory { get; set; }
        public ulong DiscordId { get; set; }

        public BattlePlayer(ulong discordId)
        {
            DiscordId = discordId;
        }

        public void InitializeCurrentPokemon(PokemonForReturnDto pokemonForReturn, PokeTypeForReturnDto pokeType, Move[] moves, int id)
        {
            CurrentPokemon = new PokeEntity(id, pokemonForReturn, pokeType, moves);
        }

        public void RewardWinningPokemonExperience(PokeEntity victim)
        {
            CurrentPokemon.Stats.GainExperience(victim);
        }


    }
}