using System;
using System.Collections.Generic;
using PokeBot.PokeBattle.Common;
using PokeBot.PokeBattle.Moves;
using PokeBot.PokeBattle.Moves.Ailments;

namespace PokeBot.PokeBattle.Entities
{
    public class PokeEntity : ICombative
    {
        public Stats Stats { get; set; }
        public HashSet<string> WeakAgainst { get; set; } // Takes double damage from, and gives half damage to
        public HashSet<string> StrongAgainst { get; set; } // Takes half damage from, and gives double damage to

        public void CombatAction(ICombative other, Move move)
        {
            if (move is Attack)
            {
                Attack(other, (Attack)move);
            }
            else if (move is Effect)
            {
                ApplyEffect(other, (Effect)move);
            }
        }

        public void ReceiveEffect(Effect move)
        {
            Stats.Skills[move.StatName].CalculateStageMultiplier(move.StatChange);
        }
        private void ApplyEffect(ICombative other, Effect move)
        {
            if (move.TargetsOther && !other.isAttackDodged(this, move))
            {
                other.ReceiveEffect(move);
            }
            else
            {
                ReceiveEffect(move);
            }
        }

        public void ReceiveAilment(Ailment ailment)
        {
        }

        private void Attack(ICombative other, Attack move)
        {
            if (!isAttackDodged((PokeEntity)other, move))
            {
                var damage = CalculateDamage(other, move);
                other.TakeDamage(damage);
            }
        }

        private int CalculateDamage(ICombative defender, Attack move)
        {
            var modifier = GetDamageModifier(defender, move);
            var numerator = (((2 * Stats.Level) / 5) + 2) * move.Power * (this.Stats.Skills["Attack"].Value / this.Stats.Skills["Defense"].Value) * modifier;
            var denominator = 50;

            var damage = ((numerator / denominator) + 2) * modifier;

            return (int)damage;
        }

        private float GetDamageModifier(ICombative defender, Attack move)
        {
            float modifier = 1f;
            if (defender.isWeakAgainst(move.Type))
            {
                modifier = 2.0f;
            }
            else if (defender.isStrongAgainst(move.Type))
            {
                modifier = 0.5f;
            }
            return modifier;
        }

        public Stats GetStats()
        {
            return Stats;
        }

        public bool isDead()
        {
            return Stats.HP <= 0;
        }

        public void TakeDamage(int damage)
        {
            Stats.HP -= damage;
        }

        public bool isAttackDodged(PokeEntity attacker, Move move) // WE are the defender/dodger
        {
            // Since our data is Gen III+ use formular for Gen III Onward -- for more info: https://bulbapedia.bulbagarden.net/wiki/Statistic#Accuracy_and_evasion
            float adjustedStages = attacker.Stats.Skills["Evasion"].StageMultiplier - this.Stats.Skills["Evasion"].StageMultiplier;
            int threshold = (int)(move.Accuracy * adjustedStages);
            //Select a random number 'r' from 1 to 255 (inclusive) and compare it to 'threshold' to determine if move hits
            Random rand = new Random();
            int r = rand.Next(1, 100);

            return r <= threshold;
        }

        public bool isEffectSuccessful(PokeEntity attacker, Effect move)
        {
            int threshold = 100 - move.EffectChance; // Anything less
            Random rand = new Random();
            int r = rand.Next(101);

            return r >= threshold;
        }

        public bool isAilmentApplied(Ailment ailment)
        {
            Random rand = new Random();
            int r = rand.Next(1, 101);

            return r > (ailment.AilmentChance);
        }

        public bool isWeakAgainst(string type)
        {
            return WeakAgainst.Contains(type);
        }

        public bool isStrongAgainst(string type)
        {
            return StrongAgainst.Contains(type);
        }
    }
}