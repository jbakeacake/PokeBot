using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle.Moves.Ailments
{
    public class DamageOverTime : Ailment
    {
        private readonly float DAMAGE = 0.125f;
        private readonly int MAX_TURNS = 4;
        private int TurnCounter { get; set; }
        public DamageOverTime(string name,
            float ailmentChance,
            int chanceToRecover) : base(name, ailmentChance, chanceToRecover)
        {
            TurnCounter = 2;
        }

        public override void ApplyAilment(PokeEntity receiver)
        {
            if (TurnCounter > 0)
            {
                int damage = (int)(receiver.GetStats().MaxHP * (DAMAGE));
                receiver.TakeDamage(damage);
                TurnCounter--;
            }
            else
            {
                RemoveFrom(receiver);
            }
        }

        public override void RemoveFrom(PokeEntity illPokemon)
        {
            if(TurnCounter != 0) return;
            TurnCounter = MAX_TURNS;
            illPokemon.RemoveAilment(Name);
        }
    }
}