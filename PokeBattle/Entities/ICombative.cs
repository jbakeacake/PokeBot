using PokeBot.PokeBattle.Common;
using PokeBot.PokeBattle.Moves;
using PokeBot.PokeBattle.Moves.Ailments;

namespace PokeBot.PokeBattle.Entities
{
    public interface ICombative
    {
        bool CombatAction(ICombative other, Move move, out string combatActionMessage);
        void TakeDamage(int damage);
        void Heal(int points);
        void ReceiveEffect(Move move);
        void ReceiveAilment(Ailment ailment);
        void SetDisabled(bool isDisabled);
        bool isDisabled();
        bool isDead();
        bool isHalfDamageFrom(string type);
        bool isHalfDamageTo(string type);
        bool isDoubleDamageFrom(string type);
        bool isDoubleDamageTo(string type);
        Stats GetStats();
        bool isAttackDodged(PokeEntity attacker, Move move);
    }
}