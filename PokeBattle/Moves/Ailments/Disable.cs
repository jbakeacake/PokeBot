using System;
using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle.Moves.Ailments
{
    public class Disable : Ailment
    {
        private float PercentageOfHealth { get; set; }
        public Disable(string name,
            float ailmentChance,
            int chanceToRecover) : base(name, ailmentChance, chanceToRecover)
        {
            PercentageOfHealth = name.Equals("trap") ? (0.0625f) : 0f;
        }

        public override void ApplyAilment(ICombative receiver)
        {
            receiver.SetDisabled(true);
            var damage = receiver.GetStats().MaxHP * PercentageOfHealth;
            receiver.TakeDamage((int)damage);
        }

        public override void RemoveFrom(PokeEntity illPokemon)
        {
            illPokemon.CurrentAilments.Remove(this.Name);
            illPokemon.Disabled = false;
        }
    }
}