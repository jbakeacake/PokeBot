using PokeBot.Dtos;

namespace PokeBot.Services.Helpers
{
    public class CurrentWanderingPokemon
    {
        public PokemonForReturnDto _pokemon { get; set; }
        public bool _isCaptured { get; set; }
        public CurrentWanderingPokemon(PokemonForReturnDto pokemon)
        {
            _pokemon = pokemon;
            _isCaptured = false;
        }

        public CurrentWanderingPokemon()
        {
            _pokemon = new PokemonForReturnDto();
            _isCaptured = true;
        }

        public bool isCaptured()
        {
            return _isCaptured;
        }

        public void SetPokemon(PokemonForReturnDto pokemon)
        {
            _pokemon = pokemon;
        }

        public void SetIsCaptured(bool isCaptured)
        {
            _isCaptured = isCaptured;
        }
    }
}