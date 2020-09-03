using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle.Moves.Ailments
{
    public class Leech : Ailment
    {
        public readonly int MAX_TURNS = 4;
        private int TurnCounter { get; set; }
        public Leech(
            string name,
            float ailmentChance,
            int chanceToRecover) : base(name, ailmentChance, chanceToRecover)
        {
            TurnCounter = 4;
        }

        public override void ApplyAilment(PokeEntity receiver)
        {
            if (TurnCounter > 0)
            {
                int damage = (int)healthLeeched(receiver);
                receiver.TakeDamage(damage);
                TurnCounter--;
            }
            else
            {
                RemoveFrom(receiver);
            }
        }

        public float healthLeeched(ICombative receiver)
        {
            float damageDealt = receiver.GetStats().MaxHP * (0.10f);
            return damageDealt;
        }

        public override void RemoveFrom(PokeEntity illPokemon)
        {
            if(TurnCounter != 0) return;
            TurnCounter = MAX_TURNS;
            illPokemon.RemoveAilment(this.Name);
        }
    }
}