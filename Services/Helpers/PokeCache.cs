using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokeBot.Controllers;
using PokeBot.Dtos;

namespace PokeBot.Services.Helpers
{
    public class PokeCache
    {
        private readonly int MAX_ID = 152;
        private readonly PokemonController _pokemonController;
        public PokeCache(PokemonController pokeController)
        {
            _pokemonController = pokeController;
        }

        public async Task<PokemonForReturnDto> GetPokemon()
        {
            PokemonForReturnDto pokemonToReturn = null;
            Random rand = new Random();
            var pokeId = rand.Next(1, MAX_ID); // 1 - 151 : ids of original pokemon
            var res = await _pokemonController.PokemonExists(pokeId);
            System.Console.WriteLine($"RES: {res}");
            if(!res)
            {
                System.Console.WriteLine("Querying PokeAPI...");
                pokemonToReturn = await _pokemonController.GetPokemonAPI(pokeId);
                System.Console.WriteLine("Query Success.");
            }
            else
            {
                pokemonToReturn = await _pokemonController.GetPokemonByPokeId(pokeId);
            }
            return pokemonToReturn; 
        }
    }
}