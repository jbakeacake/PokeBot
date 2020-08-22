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
        bool isHalfDamageFrom(string type);
        bool isHalfDamageTo(string type);
        bool isDoubleDamageFrom(string type);
        bool isDoubleDamageTo(string type);
        Stats GetStats();
        bool isAttackDodged(PokeEntity attacker, Move move);
    }
}