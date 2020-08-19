using PokeBot.PokeBattle.Common;
using PokeBot.PokeBattle.Moves;

namespace PokeBot.PokeBattle.Entities
{
    public interface ICombative
    {
        void CombatAction(ICombative other, Move move);
        void TakeDamage(int damage);
        void ReceiveEffect(Effect move);
        bool isDead();
        bool isWeakAgainst(string type);
        bool isStrongAgainst(string type);
        Stats GetStats();
        bool isAttackDodged(PokeEntity attacker, Move move);
    }
}