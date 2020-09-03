namespace PokeBot.Dtos
{
    public class PokemonForUpdateDto
    {
        public string Name { get; set; }
        public float MaxHP { get; set; }
        public float Level { get; set; }
        public float Experience { get; set; }
        public float Attack { get; set; }
        public float Defense { get; set; }
        public float SpecialAttack { get; set; }
        public float SpecialDefense { get; set; }
        public float Speed { get; set; }
    }
}