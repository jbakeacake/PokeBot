using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json.Linq;
using PokeBot.Data;
using PokeBot.Dtos;
using PokeBot.Models;

namespace PokeBot.Helpers
{
    public class Seed
    {
        private static readonly int POKEDATA_ID_RANGE = 151;
        private static readonly int MOVEDATA_ID_RANGE = 165;
        private static readonly string PokeApiUrl = "https://pokeapi.co/api/v2/";
        private static DataContext _context;
        public static async Task<bool> SeedData(DataContext context)
        {
            _context = context;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PokemonDataForCreationDto, PokemonData>();
            });
            var mapper = new Mapper(config);
            if (!context.PokeData_Tbl.Any())
            {
                for (int i = 1; i <= POKEDATA_ID_RANGE; i++)
                {
                    var data = await GetPokemonDataFromAPI(i);
                    System.Console.WriteLine($"Assigning record {i}");
                    var dataForRepo = mapper.Map<PokemonData>(data);
                    _context.PokeData_Tbl.Add(dataForRepo);
                }
            }

            return await context.SaveChangesAsync() > 0;
        }

        private static async Task<PokemonDataForCreationDto> GetPokemonDataFromAPI(int pokeId)
        {
            var pokeDataUrl = PokeApiUrl + "pokemon/" + pokeId;
            PokemonDataForCreationDto pokemonDataForCreationDto = null;
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
                            var name = dataObj["name"].ToString();
                            var base_experience = JtokenParseToFloat(dataObj["base_experience"]);
                            var moveLinks = await GetMoveLinks(dataObj["moves"], pokeId);
                            var backSpriteUrl = dataObj["sprites"]["back_default"].ToString();
                            var frontSpriteUrl = dataObj["sprites"]["front_default"].ToString();
                            var statDict = ConfigureStats(dataObj["stats"]);
                            var type = dataObj["types"].First["type"]["name"].ToString();

                            pokemonDataForCreationDto = new PokemonDataForCreationDto
                            {
                                Name = name,
                                PokeId = pokeId,
                                Base_Experience = base_experience,
                                MoveLinks = moveLinks,
                                BackSpriteUrl = backSpriteUrl,
                                FrontSpriteUrl = frontSpriteUrl,
                                MaxHP = statDict["hp"],
                                Level = 1.0f,
                                Experience = 1.0f,
                                Attack = statDict["attack"],
                                Defense = statDict["defense"],
                                SpecialAttack = statDict["special-attack"],
                                SpecialDefense = statDict["special-defense"],
                                Speed = statDict["speed"],
                                Type = type
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }

            if (pokemonDataForCreationDto == null) throw new NullReferenceException("MoveData is null. Check API call.");

            return pokemonDataForCreationDto;
        }

        private static async Task<MoveDataForCreationDto> GetMoveDataFromAPI(int moveId)
        {
            var pokeDataUrl = PokeApiUrl + "move/" + moveId;
            MoveDataForCreationDto moveDataForCreationDto = null;
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
                            var name = dataObj["name"].ToString();
                            var Accuracy = JtokenParseToFloat(dataObj["accuracy"]);
                            var Effect_Chance = JtokenParseToFloat(dataObj["effect_chance"]);
                            var AilmentName = dataObj["meta"]["ailment"]["name"].ToString();
                            var Power = JtokenParseToFloat(dataObj["power"]);
                            var PP = Int32.Parse(dataObj["pp"].ToString());
                            var Type = dataObj["type"]["name"].ToString();
                            var StatChangeName = GetStatChangeName(dataObj["stat_changes"]);
                            var StatChangeValue = GetStatChangeValue(dataObj["stat_changes"]);
                            var Target = dataObj["target"]["name"].ToString();
                            moveDataForCreationDto = new MoveDataForCreationDto
                            {
                                Name = name,
                                MoveId = moveId,
                                Accuracy = Accuracy,
                                Effect_Chance = Effect_Chance,
                                AilmentName = AilmentName,
                                Power = Power,
                                PP = PP,
                                Type = Type,
                                StatChangeName = StatChangeName,
                                StatChangeValue = StatChangeValue,
                                Target = Target
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }

            if (moveDataForCreationDto == null) throw new NullReferenceException("MoveData is null. Check API call.");

            return moveDataForCreationDto;
        }

        private static bool isJtokenEmpty(JToken token)
        {
            return String.IsNullOrEmpty(token.ToString());
        }

        private async static Task<List<MoveLink>> GetMoveLinks(JToken token, int pokeId)
        {
            var listToReturn = new List<MoveLink>();
            var elems = token.ToList();
            foreach (var elem in elems)
            {
                var moveName = elem["move"]["name"].ToString();
                var moveRecord = (await _context.MoveData_Tbl.FirstOrDefaultAsync(x => x.Name == moveName));
                if(moveRecord == null) continue;
                var moveId = moveRecord.MoveId;
                listToReturn.Add(new MoveLink { MoveId = moveId, PokemonDataId = pokeId });
            }

            return listToReturn;
        }

        private static float JtokenParseToFloat(JToken token)
        {
            return isJtokenEmpty(token) ? 0 : float.Parse(token.ToString());
        }

        private static string GetStatChangeName(JToken token)
        {
            return token.First != null ? token.First["stat"]["name"].ToString() : "none";
        }

        private static int GetStatChangeValue(JToken token)
        {
            return token.First != null ? Int32.Parse(token.First["change"].ToString()) : 0;
        }

        private static Dictionary<string, float> ConfigureStats(JToken token)
        {
            Dictionary<string, float> dictToReturn = new Dictionary<string, float>();
            token.ToList().ForEach(item =>
                dictToReturn.Add(
                    item["stat"]["name"].ToString(),
                    float.Parse(item["base_stat"].ToString())
                )
            );
            return dictToReturn;
        }
    }
}