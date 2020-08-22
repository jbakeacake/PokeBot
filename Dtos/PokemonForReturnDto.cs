using System.Collections.Generic;
using PokeBot.Models;
using PokeBot.PokeBattle.Moves;

namespace PokeBot.Dtos
{
    public class PokemonForReturnDto
    {
        public int Id { get; set; }
        public int PokeId { get; set; }
        public string Name { get; set; }
        public float MaxHP { get; set; }
        public float Level { get; set; }
        public float Base_Experience { get; set; }
        public float Experience { get; set; }
        public float Attack { get; set; }
        public float Defense { get; set; }
        public float SpecialAttack { get; set; }
        public float SpecialDefense { get; set; }
        public float Speed { get; set; }
        public string Type { get; set; }
        public Move[] moves { get; set; }
        public string Url { get; set; }

        public PokemonForReturnDto() { }
        public PokemonForReturnDto(int pokeId, string name, string url)
        {
            PokeId = pokeId;
            Name = name;
            Url = url;
        }
    }
}