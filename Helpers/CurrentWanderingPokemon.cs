using PokeBot.Dtos;

namespace PokeBot.Helpers
{
    public class CurrentWanderingPokemon
    {
        public PokemonDataForReturnDto _pokemon { get; set; }
        public bool _isCaptured { get; set; }
        public CurrentWanderingPokemon(PokemonDataForReturnDto pokemon)
        {
            _pokemon = pokemon;
            _isCaptured = false;
        }

        public CurrentWanderingPokemon()
        {
            _pokemon = new PokemonDataForReturnDto();
            _isCaptured = true;
        }

        public bool isCaptured()
        {
            return _isCaptured;
        }

        public void SetPokemon(PokemonDataForReturnDto pokemon)
        {
            _pokemon = pokemon;
        }

        public void SetIsCaptured(bool isCaptured)
        {
            _isCaptured = isCaptured;
        }
    }
}