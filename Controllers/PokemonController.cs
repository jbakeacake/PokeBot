using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json.Linq;
using PokeBot.Data;
using PokeBot.Dtos;

namespace PokeBot.Controllers
{
    public class PokemonController
    {
        private readonly int ID_RANGE = 151;
        private readonly string PokeApiUrl = "https://pokeapi.co/api/v2/pokemon/";
        private readonly string PokeBastionBotApiUrl = "https://pokeres.bastionbot.org/images/pokemon/";
        private readonly IPokeBotRepository _repo;
        private readonly IMapper _mapper;
        public PokemonController(IPokeBotRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PokemonForReturnDto> GetPokemon(int id)
        {
            var pokemonFromRepo = await _repo.GetPokemon(id);
            var pokemonForReturnDto = _mapper.Map<PokemonForReturnDto>(pokemonFromRepo);
            return pokemonForReturnDto;
        }
        public async Task<PokemonForReturnDto> GetPokemonByPokeId(int pokeId)
        {
            var pokemonFromRepo = await _repo.GetPokemonByPokeId(pokeId);
            var pokemonForReturnDto =_mapper.Map<PokemonForReturnDto>(pokemonFromRepo);
            return pokemonForReturnDto;
        }

        public async Task<PokemonForReturnDto> GetPokemonAPI(int pokeId)
        {
            var pokeDataUrl = PokeApiUrl + pokeId;
            var pokePhotoUrl = PokeBastionBotApiUrl + pokeId + ".png";
            PokemonForReturnDto pokemonForReturnDto = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(pokeDataUrl))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var pokemonData = await content.ReadAsStringAsync();

                            if (pokemonData == null) throw new NullReferenceException("PokeAPI call returned null. Check Pokemon Id or base URL being used.");

                            var dataObj = JObject.Parse(pokemonData);
                            pokemonForReturnDto = new PokemonForReturnDto(pokeId, dataObj["name"].ToString(), pokePhotoUrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }

            if (pokemonForReturnDto == null) throw new NullReferenceException("PokemonForReturnDto is null. Check API call.");

            return pokemonForReturnDto;
        }

        public async Task<PokemonForReturnDto> GetRandomPokemon()
        {
            Random rand = new Random();
            int randomId = rand.Next(1, ID_RANGE + 1);
            return await GetPokemonAPI(randomId);
        }

        public async Task<bool> PokemonExists(int pokeId)
        {
            return await _repo.PokemonExists(pokeId);
        }
    }
}