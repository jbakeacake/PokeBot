namespace PokeBot.Dtos
{
    public class PokemonForCreationDto
    {
        public int PokeId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }

        public PokemonForCreationDto(int pokeId, string name)
        {
            Name = name;
            PokeId = pokeId;
        }
    }
}