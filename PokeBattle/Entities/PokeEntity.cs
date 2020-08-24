using System;
using System.Collections.Generic;
using PokeBot.Dtos;
using PokeBot.PokeBattle.Common;
using PokeBot.PokeBattle.Moves;
using PokeBot.PokeBattle.Moves.Ailments;
using PokeBot.Utils;

namespace PokeBot.PokeBattle.Entities
{
    public class PokeEntity : ICombative
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PokeTypeForReturnDto PokeType { get; set; }
        public Stats Stats { get; set; }
        public Move[] Moves { get; set; }
        public bool Disabled { get; set; }
        public Dictionary<string, Ailment> CurrentAilments { get; set; }
        public HashSet<string> Half_Damage_From { get; set; }
        public HashSet<string> Half_Damage_To { get; set; }
        public HashSet<string> Double_Damage_From { get; set; }
        public HashSet<string> Double_Damage_To { get; set; }
        public PokeEntity(int id, PokemonForReturnDto pokemon, PokeTypeForReturnDto pokeType, Move[] moves)
        {
            Id = id;
            Name = pokemon.Name;
            Moves = moves;
            PokeType = pokeType;
            Disabled = false;
            Stats = BuildStatsFromPokemon(pokemon);
            Half_Damage_From = GetHalfDamageFromTypes(pokeType);
            Half_Damage_To = GetHalfDamageToTypes(pokeType);
            Double_Damage_From = GetDoubleDamageFromTypes(pokeType);
            Double_Damage_To = GetDoubleDamageToTypes(pokeType);
        }
        private Stats BuildStatsFromPokemon(PokemonForReturnDto pokemon)
        {
            return new StatsBuilder()
                .HP(pokemon.MaxHP)
                .Level(pokemon.Level)
                .Base_Experience(pokemon.Base_Experience)
                .Experience(pokemon.Experience)
                .Attack(pokemon.Attack)
                .Defense(pokemon.Defense)
                .SpecialAttack(pokemon.SpecialAttack)
                .SpecialDefense(pokemon.SpecialDefense)
                .Speed(pokemon.Speed)
                .Accuracy(1.0f) //Accuracy always starts at 100%
                .Evasion(1.0f)
                .Build();
        }
        private HashSet<string> GetDoubleDamageToTypes(PokeTypeForReturnDto type)
        {
            HashSet<string> setToReturn = new HashSet<string>();
            foreach (var t in type.Double_Damage_To)
            {
                setToReturn.Add(t);
            }
            return setToReturn;
        }

        private HashSet<string> GetDoubleDamageFromTypes(PokeTypeForReturnDto type)
        {
            HashSet<string> setToReturn = new HashSet<string>();
            foreach (var t in type.Double_Damage_From)
            {
                setToReturn.Add(t);
            }
            return setToReturn;
        }

        private HashSet<string> GetHalfDamageToTypes(PokeTypeForReturnDto type)
        {
            HashSet<string> setToReturn = new HashSet<string>();
            foreach (var t in type.Half_Damage_To)
            {
                setToReturn.Add(t);
            }
            return setToReturn;
        }

        private HashSet<string> GetHalfDamageFromTypes(PokeTypeForReturnDto type)
        {
            HashSet<string> setToReturn = new HashSet<string>();
            foreach (var t in type.Half_Damage_From)
            {
                setToReturn.Add(t);
            }
            return setToReturn;
        }

        public void CombatAction(ICombative other, Move move)
        {
            Attack(other, move);
        }

        public void ReceiveEffect(Move move)
        {
            Stats.Skills[move.StatChangeName].CalculateStageMultiplier(move.StatChangeValue);
        }
        public void TryToRecoverAilments()
        {
            foreach(var ailment in CurrentAilments.Values)
            {
                if(ailment.IsRecoverySuccessful())
                {
                    CurrentAilments.Remove(ailment.Name);
                    System.Console.WriteLine($"{Name} recovered from {ailment.Name}");
                }
            }
        }
        private void ApplyEffect(ICombative other, Move move)
        {
            if (move.TargetsOther 
                && !other.isAttackDodged(this, move)
                && isEffectSuccessful(this, move))
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
            if(CurrentAilments.ContainsKey(ailment.Name)) return;
            CurrentAilments.Add(ailment.Name, ailment);
        }

        private void Attack(ICombative other, Move move)
        {
            if (!isAttackDodged((PokeEntity)other, move))
            {
                var damage = CalculateDamage(other, move);
                other.TakeDamage(damage);
            }
        }

        private int CalculateDamage(ICombative defender, Move move)
        {
            var modifier = GetDamageModifier(defender, move);
            string attackSkill = isSpecialAttack(move.Type) ? "SpecialAttack" : "Attack";
            string defenseSkill = isSpecialAttack(move.Type) ? "SpecialDefense" : "Defense";
            var numerator = (((2 * Stats.Level) / 5) + 2) * move.Power * (this.Stats.Skills[attackSkill].Value / defender.GetStats().Skills[defenseSkill].Value) * modifier;
            var denominator = 50;

            var damage = ((numerator / denominator) + 2) * modifier;

            return (int)damage;
        }

        private float GetDamageModifier(ICombative defender, Move move)
        {
            float modifier = 1f;
            if (defender.isDoubleDamageFrom(move.Type))
            {
                modifier = 2.0f;
            }
            else if (defender.isHalfDamageTo(move.Type))
            {
                modifier = 0.5f;
            }
            return modifier;
        }

        private bool isSpecialAttack(string type)
        {
            return SpecialMoveTypeSetUtils.GetSpecialMoveTypeSet.Contains(type);
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

        public bool isEffectSuccessful(PokeEntity attacker, Move move)
        {
            float threshold = 100f - move.EffectChance; // Anything less
            Random rand = new Random();
            float r = rand.Next(101);

            return r >= threshold;
        }

        public bool isAilmentApplied(Ailment ailment)
        {
            Random rand = new Random();
            int r = rand.Next(1, 101);

            return r > (ailment.AilmentChance);
        }

        public bool isHalfDamageFrom(string type)
        {
            return Half_Damage_From.Contains(type);
        }

        public bool isHalfDamageTo(string type)
        {
            return Half_Damage_To.Contains(type);
        }
        public bool isDoubleDamageFrom(string type)
        {
            return Double_Damage_From.Contains(type);
        }

        public bool isDoubleDamageTo(string type)
        {
            return Double_Damage_To.Contains(type);
        }

        public void SetDisabled(bool isDisabled)
        {
            Disabled = isDisabled;
        }

        public bool isDisabled()
        {
            return Disabled;
        }
    }
}