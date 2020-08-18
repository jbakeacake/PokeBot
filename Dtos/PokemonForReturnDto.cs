namespace PokeBot.Dtos
{
    public class PokemonForReturnDto
    {
        public int PokeId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public PokemonForReturnDto() {}
        public PokemonForReturnDto(int pokeId, string name, string url)
        {
            PokeId = pokeId;
            Name = name;
            Url = url;
        }
    }
}