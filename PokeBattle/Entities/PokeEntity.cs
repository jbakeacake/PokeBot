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
        public int PokeId { get; set; }
        public string Name { get; set; }
        public string BackSpriteUrl { get; set; }
        public string FrontSpriteUrl { get; set; }
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
            PokeId = pokemon.PokeId;
            Name = pokemon.Name;
            Moves = moves;
            PokeType = pokeType;
            Disabled = false;
            CurrentAilments = new Dictionary<string, Ailment>();
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
            if (move.StatChangeValue < 0)
            {
                System.Console.WriteLine($"{Name}'s {move.StatChangeName} fell by {move.StatChangeValue}. Now {Stats.Skills[move.StatChangeName].Stage}");
            }
            else
            {
                System.Console.WriteLine($"{Name}'s {move.StatChangeName} rose by {move.StatChangeValue}. Now {Stats.Skills[move.StatChangeName].Stage}");
            }
        }
        private void ApplyEffect(ICombative other, Move move)
        {
            if (move.TargetsOther
                && isEffectSuccessful(this, move))
            {
                other.ReceiveEffect(move);
            }
            else
            {
                ReceiveEffect(move);
            }
        }
        private void Attack(ICombative other, Move move)
        {
            System.Console.WriteLine($"{Name} used {move.Name}!");
            if (!isAttackDodged((PokeEntity)other, move))
            {
                var damage = CalculateDamage(other, move);
                other.TakeDamage(damage);

                if (!move.StatChangeName.Equals("none"))
                {
                    System.Console.WriteLine($"{Name} applying {move.Name} to other.");
                    ApplyEffect(other, move);
                }
                if (move.Ailment != null)
                {
                    System.Console.WriteLine($"{Name} trying to apply {move.Ailment.Name}");
                    other.ReceiveAilment(move.Ailment);
                }
            }
            else
            {
                System.Console.WriteLine("It missed!");
            }
        }

        private int CalculateDamage(ICombative defender, Move move)
        {
            var modifier = GetDamageModifier(defender, move);
            string attackSkill = isSpecialAttack(move.Type) ? "special-attack" : "attack";
            string defenseSkill = isSpecialAttack(move.Type) ? "special-defense" : "defense";
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
            System.Console.WriteLine($"{Name}: {Stats.HP} / {Stats.MaxHP}");
            return Stats.HP <= 0;
        }

        public void TakeDamage(int damage)
        {
            Stats.HP -= damage;
            System.Console.WriteLine($"{Name} took {damage} points of damage!");
        }
        public void Heal(int points)
        {
            var updatedHP = Stats.HP += points;
            if (updatedHP >= Stats.MaxHP)
            {
                Stats.HP = Stats.MaxHP;
            }
            else
            {
                Stats.HP = updatedHP;
            }
        }
        public bool isAttackDodged(PokeEntity attacker, Move move) // WE are the defender/dodger
        {
            // Since our data is Gen III+ use formular for Gen III Onward -- for more info: https://bulbapedia.bulbagarden.net/wiki/Statistic#Accuracy_and_evasion
            float adjustedStages = this.Stats.Skills["evasion"].Stage - attacker.Stats.Skills["accuracy"].Stage;
            float threshold = (move.Accuracy * adjustedStages);
            //Select a random number 'r' from 1 to 255 (inclusive) and compare it to 'threshold' to determine if move hits
            Random rand = new Random();
            float r = rand.Next(1, 101);
            return r <= threshold;
        }

        public bool isEffectSuccessful(PokeEntity attacker, Move move)
        {
            float threshold = move.EffectChance; // Anything less
            Random rand = new Random();
            float r = rand.Next(101);
            return r <= threshold;
        }
        public void ReceiveAilment(Ailment ailment)
        {
            if (CurrentAilments.ContainsKey(ailment.Name)
                || !isAilmentApplied(ailment)) return;

            System.Console.WriteLine($"{Name} was afflicted with {ailment.Name}");
            CurrentAilments.Add(ailment.Name, ailment);
        }
        public void TriggerAilments(ICombative attacker)
        {
            if (CurrentAilments.Count == 0) return;
            foreach (var ailment in CurrentAilments.Values)
            {
                ailment.ApplyAilment(this);
                if (ailment.GetType().Equals(typeof(Leech)))
                {
                    var healthLeeched = ((Leech)ailment).healthLeeched(this);
                    attacker.Heal((int)healthLeeched);
                }
            }
        }
        public void TryToRecoverAilments()
        {
            foreach (var ailment in CurrentAilments.Values)
            {
                System.Console.WriteLine($"{Name} trying to recover from {ailment.Name}.");
                if (ailment.IsRecoverySuccessful())
                {
                    ailment.RemoveFrom(this);
                    CurrentAilments.Remove(ailment.Name);
                    System.Console.WriteLine($"{Name} recovered from {ailment.Name}");
                }
            }
        }
        private bool isAilmentApplied(Ailment ailment)
        {
            Random rand = new Random();
            int r = rand.Next(1, 101);

            return r < (ailment.AilmentChance);
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