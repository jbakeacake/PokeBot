using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle.Moves.Ailments
{
    public class DamageOverTime : Ailment
    {
        private readonly float DAMAGE = 0.125f;
        public DamageOverTime(string name, 
            float ailmentChance,
            int chanceToRecover) : base(name, ailmentChance, chanceToRecover)
        {
        }

        public override void ApplyAilment(ICombative receiver)
        {
            int damage = (int)(receiver.GetStats().MaxHP * (DAMAGE));
            receiver.TakeDamage(damage);
        }

        public override void RemoveFrom(PokeEntity illPokemon)
        {
            illPokemon.CurrentAilments.Remove(this.Name);
        }
    }
}