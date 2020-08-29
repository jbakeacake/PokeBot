using System;
using System.Collections.Generic;
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
        public Dictionary<string, int> LevelUpValues { get; set; }
        public BattlePlayer(ulong discordId)
        {
            DiscordId = discordId;
        }

        public void InitializeCurrentPokemon(PokemonForReturnDto pokemonForReturn, PokeTypeForReturnDto pokeType, Move[] moves, int id)
        {
            CurrentPokemon = new PokeEntity(id, pokemonForReturn, pokeType, moves);
        }

        public void RewardWinningPokemonExperience(PokeEntity loser)
        {
            CurrentPokemon.Stats.GainExperience(loser);
        }

        public void RewardLosingPokemonExperience(PokeEntity loser)
        {
            CurrentPokemon.Stats.GainHalfExperience(loser);
        }

        public void DeterminePokemonLevelUp()
        {
            while(CurrentPokemon.Stats.CanLevelUp())
            {
                System.Console.WriteLine($"{CurrentPokemon.Name} level'd up!");
                CurrentPokemon.Stats.LevelUp();
            }
        }
    }
}