using System.Collections.Generic;
using PokeBot.Models;

namespace PokeBot.Dtos
{
    public class PokemonForCreationDto
    {
        public int PokeId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
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
        public int MoveId_One { get; set; }
        public int MoveId_Two { get; set; }
        public int MoveId_Three { get; set; }
        public int MoveId_Four { get; set; }

        public PokemonForCreationDto(int pokeId, string name)
        {
            Name = name;
            PokeId = pokeId;
        }

        public PokemonForCreationDto()
        { }
    }
}