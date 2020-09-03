namespace PokeBot.Models
{
    public class MoveLink
    {
        public int Id { get; set; }
        public int MoveId { get; set; }
        public PokemonData PokemonData { get; set; }
        public int PokemonDataId { get; set; }
    }
}