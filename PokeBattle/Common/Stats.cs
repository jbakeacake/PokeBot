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
        public void GainExperience(PokeEntity victim)
        {
            // Based on the formula for XP gain for a pokemon -- for more info see: https://bulbapedia.bulbagarden.net/wiki/Experience#Experience_gain_in_battle
            float a = 1.5f;
            float b = victim.Stats.Base_Experience;
            float e = 1f;
            float f = 1f;
            float L = victim.Stats.Level;
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

        private void LevelUp()
        {
            float skillModifier = 0.05f;
            foreach(var key in Skills.Keys)
            {
                var pointsToAdd = Skills[key].Value * skillModifier;
                Skills[key].Value += pointsToAdd;
            }
            //Increase Health:
            float hpModifier = 0.10f;
            var hpToAdd = MaxHP * hpModifier;
            
            MaxHP += hpToAdd;
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