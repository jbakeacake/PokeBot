using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle.Moves.Ailments
{
    public class Leech : Ailment
    {
        public Leech(
            string name,
            string description,
            float ailmentChance,
            int chanceToRecover) : base(name, description, ailmentChance, chanceToRecover)
        {
        }

        public override void ApplyAilment(ICombative receiver)
        {
            int damage = (int)healthLeeched(receiver);
            receiver.TakeDamage(damage);
        }

        private float healthLeeched(ICombative receiver)
        {
            float damageDealt = receiver.GetStats().MaxHP * (0.125f);
            return damageDealt;
        }
    }
}