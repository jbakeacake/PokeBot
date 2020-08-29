using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle.Moves.Ailments
{
    public class Confusion : Ailment
    {
        public Confusion(string name, float ailmentChance, int chanceToRecover) : base(name, ailmentChance, chanceToRecover)
        {
        }

        public override void ApplyAilment(PokeEntity receiver)
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveFrom(PokeEntity illPokemon)
        {
            throw new System.NotImplementedException();
        }
    }
}