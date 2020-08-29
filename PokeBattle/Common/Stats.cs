using System;
using System.Collections.Generic;
using PokeBot.PokeBattle.Entities;
using PokeBot.PokeBattle.Moves;

namespace PokeBot.PokeBattle.Common
{
    public class Stats
    {
        public float MaxHP { get; set; }
        public float HP { get; set; }
        public float Level { get; set; }
        public float Base_Experience { get; set; }
        public float Experience { get; set; }
        public Dictionary<string, Skill> Skills; // Note: Implement class for each skill to prevent overly debuffing pokemon

        public Stats()
        {
            Skills = new Dictionary<string, Skill>();
        }
        public override string ToString()
        {
            var message = "";
            message += $"Max HP: {((int)MaxHP)}\n";
            message += $"Level: {Level}\n";
            message += $"EXP: {Experience} / {CalculateExperienceNeededForNextLevel()}\n";
            message += SkillsToString();

            return message;
        }
        private string SkillsToString()
        {
            var message = "";
            foreach (var record in Skills)
            {
                message += $"{record.Key}: {((int) record.Value.Value)}\n";
            }
            return message;
        }
        public void GainExperience(PokeEntity loser)
        {
            // Based on the formula for XP gain for a pokemon -- for more info see: https://bulbapedia.bulbagarden.net/wiki/Experience#Experience_gain_in_battle
            float a = 1.5f;
            float b = loser.Stats.Base_Experience;
            float e = 1f;
            float f = 1f;
            float L = loser.Stats.Level;
            float p = 1f;
            float s = 1f;
            float t = 1f;
            float v = 1f;

            float numerator = a * t * b * e * L * p * f * v;
            float denominator = 7 * s;

            int gain = (int)(numerator / denominator);
            System.Console.WriteLine($"EXP GAIN: {gain}");
            this.Experience += gain;
        }

        public void GainHalfExperience(PokeEntity loser)
        {
            // Based on the formula for XP gain for a pokemon -- for more info see: https://bulbapedia.bulbagarden.net/wiki/Experience#Experience_gain_in_battle
            float a = 1.5f;
            float b = loser.Stats.Base_Experience;
            float e = 1f;
            float f = 1f;
            float L = loser.Stats.Level;
            float p = 1f;
            float s = 1f;
            float t = 1f;
            float v = 1f;

            float numerator = a * t * b * e * L * p * f * v;
            float denominator = 7 * s;

            int gain = (int)(numerator / denominator);
            System.Console.WriteLine($"EXP GAIN: {gain}");
            this.Experience += (gain/2);
        }

        public void LevelUp()
        {
            float skillModifier = 0.05f;
            foreach (var key in Skills.Keys)
            {
                var pointsToAdd = ((float) Skills[key].Value * skillModifier);
                Skills[key].Value += pointsToAdd;
            }
            //Increase Health:
            float hpModifier = 0.10f;
            var hpToAdd = MaxHP * hpModifier;

            MaxHP += ((float)Math.Ceiling(hpToAdd));
            Level += 1;
        }
        public bool CanLevelUp()
        {
            return Experience >= CalculateExperienceNeededForNextLevel();
        }
        public int CalculateExperienceNeededForNextLevel()
        {
            double nextLevel = Level + 1;
            double experienceNeeded = Math.Pow(nextLevel, 3.0);

            return (int)experienceNeeded;
        }
    }
}