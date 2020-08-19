using System.Collections.Generic;

namespace PokeBot.Models
{
    public class PokemonData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PokeId { get; set; }
        public float Base_Experience { get; set; }
        public ICollection<MoveLink> MoveLinks { get; set; }
        public string BackSpriteUrl { get; set; }
        public string FrontSpriteUrl { get; set; }
        public float MaxHP { get; set; }
        public float Level { get; set; }
        public float Experience { get; set; }
        public float Attack { get; set; }
        public float Defense { get; set; }
        public float SpecialAttack { get; set; }
        public float SpecialDefense { get; set; }
        public float Speed { get; set; }
        public string Type { get; set; }
    }
}