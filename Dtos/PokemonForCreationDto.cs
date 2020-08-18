namespace PokeBot.Dtos
{
    public class PokemonForCreationDto
    {
        public int PokeId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string Url { get; set; }

        public PokemonForCreationDto(int pokeId, string name, string url)
        {
            Name = name;
            PokeId = pokeId;
            Url = url;
        }
    }
}