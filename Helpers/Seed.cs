using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private static readonly int POKETYPE_ID_RANGE = 18;
        private static readonly string PokeApiUrl = "https://pokeapi.co/api/v2/";
        private static DataContext _context;
        public static async Task<bool> SeedPokeData(DataContext context)
        {
            _context = context;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PokemonDataForCreationDto, PokemonData>();
            });
            var mapper = new Mapper(config);
            if (!(context.PokeData_Tbl.Any()))
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

        public static async Task<bool> SeedPokeTypes(DataContext context)
        {
            if(!context.PokeType_Tbl.Any())
            {
                for(int i = 1; i <= POKETYPE_ID_RANGE; i++)
                {
                    var pokeTypeForRepo = await GetPokeTypesFromAPI(i);
                    context.PokeType_Tbl.Add(pokeTypeForRepo);
                    System.Console.WriteLine($"Seeding type {i}");
                }
            }
            return await context.SaveChangesAsync() > 0;
        }        

        public static async Task<bool> SeedRandomStats(DataContext context)
        {
            var usersPokemon = context.Pokemon_Tbl.ToList();
            foreach(var pokemon in usersPokemon)
            {
                var basePokeData = await context.PokeData_Tbl.Include(p => p.MoveLinks).FirstOrDefaultAsync(x => x.PokeId == pokemon.PokeId);
                SetStats(pokemon, basePokeData);
                SetRandomMoveIds(context, pokemon, basePokeData);
                pokemon.Type = basePokeData.Type;
                pokemon.Level = 1.0f;
                pokemon.Base_Experience = basePokeData.Base_Experience;
                System.Console.WriteLine($"Assigning to {pokemon.Name}");
            }

            return await context.SaveChangesAsync() > 0;
        }

        private static void SetStats(Pokemon pokemon, PokemonData data)
        {
            pokemon.MaxHP = GetRandomStat(data.MaxHP);
            pokemon.Attack = GetRandomStat(data.Attack);
            pokemon.Defense = GetRandomStat(data.Defense);
            pokemon.SpecialAttack = GetRandomStat(data.SpecialAttack);
            pokemon.SpecialDefense = GetRandomStat(data.SpecialDefense);
            pokemon.Speed = GetRandomStat(data.Speed);
        }

        private async static void SetRandomMoveIds(DataContext context, Pokemon pokemon, PokemonData data)
        {
            int[] arrOfMoveIds = new int[4];

            Random rand = new Random();
            var pokeData = await context.PokeData_Tbl.Include(x => x.MoveLinks).AsQueryable().FirstOrDefaultAsync(x => x.PokeId == pokemon.PokeId);
            var moveCount = pokeData.MoveLinks.Count();
            int skips = rand.Next(1, moveCount-1);
            for(int i = 0; i < arrOfMoveIds.Length; i++)
            {
                if(pokeData.MoveLinks.Count() == 0) throw new Exception("EMPTY MOVE LINK FROM POKE DATA");
                arrOfMoveIds[i] = pokeData.MoveLinks.Skip(skips).Take(1).FirstOrDefault().MoveId;
                skips = rand.Next(1, moveCount-1);
            }

            pokemon.MoveId_One = arrOfMoveIds[0];
            pokemon.MoveId_Two = arrOfMoveIds[1];
            pokemon.MoveId_Three = arrOfMoveIds[2];
            pokemon.MoveId_Four = arrOfMoveIds[3];
        }

        public static async Task<bool> SeedMoveData(DataContext context, IMapper mapper)
        {
            for(int i = 1; i < MOVEDATA_ID_RANGE; i++)
            {
                MoveDataForCreationDto md = await GetMoveDataFromAPI(i);
                var mdForCreation = mapper.Map<MoveData>(md);
                context.MoveData_Tbl.Add(mdForCreation);
            }

            return await context.SaveChangesAsync() > 0;
        }


        private static float GetRandomStat(float base_stat)
        {
            Random rand = new Random();
            return base_stat + (float)(rand.Next(1, 20));
        }
        private static async Task<PokeType> GetPokeTypesFromAPI(int typeId)
        {
            var pokeTypeUrl = PokeApiUrl + "type/" + typeId;
            PokeType pokeType = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(pokeTypeUrl))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var pokemonData = await content.ReadAsStringAsync();
                            if (pokemonData == null) throw new NullReferenceException("PokeAPI call returned null. Check Pokemon Id or Url");

                            var dataObj = JObject.Parse(pokemonData);
                            var name = dataObj["name"].ToString();
                            var delimitedDoubleDamageFrom = GetPokeTypes(dataObj["damage_relations"], "double_damage_from");
                            var delimitedDoubleDamageTo = GetPokeTypes(dataObj["damage_relations"], "double_damage_to");
                            var delimitedHalfDamageFrom = GetPokeTypes(dataObj["damage_relations"], "half_damage_from");
                            var delimitedHalfDamageTo = GetPokeTypes(dataObj["damage_relations"], "half_damage_to");

                            pokeType = new PokeType
                            {
                                Name = name,
                                PokeTypeId = typeId,
                                Delimited_Double_Damage_From = delimitedDoubleDamageFrom,
                                Delimited_Double_Damage_To = delimitedDoubleDamageTo,
                                Delimited_Half_Damage_From = delimitedHalfDamageFrom,
                                Delimited_Half_Damage_To = delimitedHalfDamageTo
                            };
                        }
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }

            if(pokeType == null) throw new NullReferenceException("PokeType is null; check PokeType construction after API call");

            return pokeType;
        }

        private static string GetPokeTypes(JToken jTokens, string v)
        {
            if(jTokens[v].Count() == 0) return "none";
            string strToReturn = "";
            jTokens[v].ToList().ForEach(elem => {
                strToReturn += (elem["name"] + ",");
            });
            
            strToReturn = strToReturn.Substring(0, strToReturn.Length-1); // remove trailing comma
            return strToReturn;
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
                var moveRecord = await _context.MoveData_Tbl.AsQueryable().FirstAsync();
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