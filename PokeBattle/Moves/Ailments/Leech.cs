using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle.Moves.Ailments
{
    public class Leech : Ailment
    {
        public Leech(
            string name,
            float ailmentChance,
            int chanceToRecover) : base(name, ailmentChance, chanceToRecover)
        {
        }

        public override void ApplyAilment(ICombative receiver)
        {
            int damage = (int)healthLeeched(receiver);
            receiver.TakeDamage(damage);
        }

        public float healthLeeched(ICombative receiver)
        {
            float damageDealt = receiver.GetStats().MaxHP * (0.125f);
            return damageDealt;
        }

        public override void RemoveFrom(PokeEntity illPokemon)
        {
            illPokemon.CurrentAilments.Remove(this.Name);
        }
    }
}