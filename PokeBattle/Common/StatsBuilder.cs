namespace PokeBot.PokeBattle.Common
{
    public class StatsBuilder
    {
        private Stats _stats { get; set; }
        public StatsBuilder() {
            _stats = new Stats();
        }
        public Stats Build()
        {
            return _stats;
        }

        public StatsBuilder HP(float hp)
        {
            _stats.MaxHP = hp;
            _stats.HP = hp;
            return this;
        }
        public StatsBuilder Base_Experience(float base_Experience)
        {
            _stats.Base_Experience = base_Experience;
            return this;
        }
        public StatsBuilder Level(float level)
        {
            _stats.Level = level;
            return this;
        }
        public StatsBuilder Experience(float experience)
        {
            _stats.Experience = experience;
            return this;
        }
        public StatsBuilder Attack(float attack)
        {
            var skill = new Skill("attack", attack, 0, 1.0f);
            _stats.Skills.Add("attack", skill);
            return this;
        }
        public StatsBuilder Defense(float defense)
        {
            var skill = new Skill("defense", defense, 0, 1.0f);
            _stats.Skills.Add("defense", skill);
            return this;
        }
        public StatsBuilder SpecialAttack(float specialAttack)
        {
            var skill = new Skill("special-attack", specialAttack, 0, 1.0f);
            _stats.Skills.Add("special-attack", skill);
            return this;
        }
        public StatsBuilder SpecialDefense(float specialDefense)
        {
            var skill = new Skill("special-defense", specialDefense, 0, 1.0f);
            _stats.Skills.Add("special-defense", skill);
            return this;
        }
        public StatsBuilder Speed(float speed)
        {
            var skill = new Skill("speed", speed, 0, 1.0f);
            _stats.Skills.Add("speed", skill);
            return this;
        }
        public StatsBuilder Accuracy(float accuracy)
        {
            var skill = new Skill("accuracy", accuracy, 0, 1.0f);
            _stats.Skills.Add("accuracy", skill);
            return this;
        }
        public StatsBuilder Evasion(float evasion)
        {
            var skill = new Skill("evasion", evasion, 0, 1.0f);
            _stats.Skills.Add("evasion", skill);
            return this;
        }
    }
}