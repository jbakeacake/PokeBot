using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json.Linq;
using PokeBot.Data;
using PokeBot.Dtos;
using PokeBot.Models;

namespace PokeBot.Controllers
{
    public class PokemonController
    {
        private readonly int ID_RANGE = 151;
        private readonly string PokeApiUrl = "https://pokeapi.co/api/v2/pokemon/";
        private readonly string PokeBastionBotApiUrl = "https://pokeres.bastionbot.org/images/pokemon/";
        private readonly IPokeRepository _repo;
        private readonly IMapper _mapper;
        public PokemonController(IPokeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task RegisterPokeBattle(PokeBattleForCreationDto pokeBattleForCreationDto)
        {
            PokeBattleData battleDataForRepo = _mapper.Map<PokeBattleData>(pokeBattleForCreationDto);
            _repo.Add(battleDataForRepo);
            var res = await _repo.SaveAll();
            if(!res) throw new Exception($"Failed to register new PokeBattle to database. Please check battle data construction.");

            System.Console.WriteLine($"PokeBattle Id {battleDataForRepo.BattleTokenId} successfully added.");
        }

        public async Task<PokemonDataForReturnDto> GetPokemonData(int pokeId)
        {
            var pokemonDataFromRepo = await _repo.GetPokemonDataByPokeId(pokeId);
            var pokemonDataForReturn = _mapper.Map<PokemonDataForReturnDto>(pokemonDataFromRepo);
            return pokemonDataForReturn;
        }

        public async Task<PokemonDataForReturnDto> GetRandomPokemonData()
        {
            Random rand = new Random();
            int randomId = rand.Next(1, ID_RANGE + 1);
            var pokemonFromRepo = await _repo.GetPokemonDataByPokeId(randomId);
            var pokemonDataForReturn = _mapper.Map<PokemonDataForReturnDto>(pokemonFromRepo);
            return pokemonDataForReturn;
        }

        public async Task<PokemonForReturnDto> GetPokemonFromUserInventory(int id)
        {
            var pokemonFromRepo = await _repo.GetPokemon(id);
            var pokemonForReturn = _mapper.Map<PokemonForReturnDto>(pokemonFromRepo);
            return pokemonForReturn;
        }

        public async Task<PokeTypeForReturnDto> GetPokeType(string type)
        {
            var pokeTypeFromRepo = await _repo.GetPokeTypeByName(type);
            var pokeTypeForReturn = _mapper.Map<PokeTypeForReturnDto>(pokeTypeFromRepo);
            return pokeTypeForReturn;
        }
    }
}