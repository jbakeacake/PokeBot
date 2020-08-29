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

        public bool CombatAction(ICombative other, Move move, out string combatActionMessage)
        {
            combatActionMessage = "";
            if (move.PP == 0) return false;
            combatActionMessage += Attack(other, move);
            move.PP--;
            return true;
        }

        public void ReceiveEffect(Move move)
        {
            Stats.Skills[move.StatChangeName].CalculateStageMultiplier(move.StatChangeValue);
        }
        private void ApplyEffect(ICombative other, Move move)
        {
            if (move.TargetsOther
                && isEffectSuccessful(this, move))
            {
                other.ReceiveEffect(move);
            }
            else if (!move.TargetsOther)
            {
                ReceiveEffect(move);
            }
        }
        private string Attack(ICombative other, Move move)
        {
            if (!other.isAttackDodged(this, move))
            {
                var damage = CalculateDamage(other, move);
                other.TakeDamage(damage);

                if (!move.StatChangeName.Equals("none"))
                {
                    ApplyEffect(other, move);
                }
                if (move.Ailment != null)
                {
                    other.ReceiveAilment(move.Ailment);
                }
                return CreateEffectiveMoveMessage(other, move);
            }
            else
            {
                return "|It missed!\n";
            }
        }

        private string CreateEffectiveMoveMessage(ICombative other, Move move)
        {
            if (other.isDoubleDamageFrom(move.Type))
            {
                return "|It's super effective!\n";
            }
            else if (other.isHalfDamageFrom(move.Type))
            {
                return "|It's not very effective...\n";
            }
            else
            {
                return "";
            }
        }
        private int CalculateDamage(ICombative defender, Move move)
        {
            var modifier = GetDamageModifier(defender, move);
            float stageMultiplier = isSpecialAttack(move.Type) ? this.Stats.Skills["special-attack"].StageMultiplier : this.Stats.Skills["attack"].StageMultiplier;
            string attackSkill = isSpecialAttack(move.Type) ? "special-attack" : "attack";
            string defenseSkill = isSpecialAttack(move.Type) ? "special-defense" : "defense";
            var numerator = (((2 * Stats.Level) / 5) + 2) * move.Power * (this.Stats.Skills[attackSkill].Value / defender.GetStats().Skills[defenseSkill].Value) * modifier * stageMultiplier;
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
            var defenderEvasionMultiplier = this.Stats.Skills["evasion"].StageMultiplier;
            var attackerAccuracyMultiplier = attacker.Stats.Skills["accuracy"].StageMultiplier;
            float adjustedStages = defenderEvasionMultiplier - attackerAccuracyMultiplier;
            System.Console.WriteLine($"Adjusted Stages : {defenderEvasionMultiplier} - {attackerAccuracyMultiplier}");
            float threshold = (move.Accuracy * adjustedStages);
            //Select a random number 'r' from 1 to 255 (inclusive) and compare it to 'threshold' to determine if move hits
            Random rand = new Random();
            float r = rand.Next(1, 101);
            System.Console.WriteLine($"ATTACK ROLL: r: {r} <= T: {threshold}");
            return r <= threshold;
        }

        public bool isEffectSuccessful(PokeEntity attacker, Move move)
        {
            float threshold = move.EffectChance; // Anything less
            if(threshold == 0) return true;

            Random rand = new Random();
            float r = rand.Next(101);
            System.Console.WriteLine($"EFFECT ROLL: r: {r} <= T: {threshold}");
            return r <= threshold;
        }
        public void ReceiveAilment(Ailment ailment)
        {
            if (CurrentAilments.ContainsKey(ailment.Name)
                || !isAilmentApplied(ailment)) return;

            System.Console.WriteLine($"{Name} was afflicted with {ailment.Name}");
            CurrentAilments.Add(ailment.Name, ailment);
        }
        public string TriggerAilments(ICombative attacker)
        {
            var logMessage = "";
            if (CurrentAilments.Count == 0) return "";
            foreach (var ailment in CurrentAilments.Values)
            {
                ailment.ApplyAilment(this);
                if (ailment.GetType().Equals(typeof(Leech)))
                {
                    var healthLeeched = ((Leech)ailment).healthLeeched(this);
                    attacker.Heal((int)healthLeeched);
                }
                if(CurrentAilments.ContainsKey(ailment.Name))
                    logMessage += $"|{Name} was afflicted by {ailment.Name}! \n";
            }
            return logMessage;
        }
        public void RemoveAilment(string name)
        {
            System.Console.WriteLine($"{name} removed!");
            CurrentAilments.Remove(name);
        }
        private bool isAilmentApplied(Ailment ailment)
        {
            Random rand = new Random();
            int r = rand.Next(1, 101);
            System.Console.WriteLine($"r: {r} < T: {ailment.AilmentChance}");
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