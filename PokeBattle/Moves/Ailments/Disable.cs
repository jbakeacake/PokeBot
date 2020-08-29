using System;
using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle.Moves.Ailments
{
    public class Disable : Ailment
    {
        private float PercentageOfHealth { get; set; }
        private readonly int MAX_TURNS = 2;
        private int TurnCounter { get; set; }
        public Disable(string name,
            float ailmentChance,
            int chanceToRecover) : base(name, ailmentChance, chanceToRecover)
        {
            PercentageOfHealth = name.Equals("trap") ? (0.0625f) : 0f;
            TurnCounter = MAX_TURNS;
        }

        public override void ApplyAilment(PokeEntity receiver)
        {
            if (TurnCounter > 0)
            {
                receiver.SetDisabled(true);
                var damage = receiver.GetStats().MaxHP * PercentageOfHealth;
                receiver.TakeDamage((int)damage);
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
            illPokemon.CurrentAilments.Remove(this.Name);
            TurnCounter = MAX_TURNS;
            illPokemon.SetDisabled(false);
        }
    }
}