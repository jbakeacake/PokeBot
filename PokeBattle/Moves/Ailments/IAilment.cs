using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle.Moves.Ailments
{
    public interface IAilment
    {
        void ApplyAilment(PokeEntity receiver);
    }
}