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
            var skill = new Skill("Attack", attack, 0, 1.0f);
            _stats.Skills.Add("Attack", skill);
            return this;
        }
        public StatsBuilder Defense(float defense)
        {
            var skill = new Skill("Defense", defense, 0, 1.0f);
            _stats.Skills.Add("Defense", skill);
            return this;
        }
        public StatsBuilder SpecialAttack(float specialAttack)
        {
            var skill = new Skill("SpecialAttack", specialAttack, 0, 1.0f);
            _stats.Skills.Add("SpecialAttack", skill);
            return this;
        }
        public StatsBuilder SpecialDefense(float specialDefense)
        {
            var skill = new Skill("SpecialDefense", specialDefense, 0, 1.0f);
            _stats.Skills.Add("SpecialDefense", skill);
            return this;
        }
        public StatsBuilder Speed(float speed)
        {
            var skill = new Skill("Speed", speed, 0, 1.0f);
            _stats.Skills.Add("Speed", skill);
            return this;
        }
        public StatsBuilder Accuracy(float accuracy)
        {
            var skill = new Skill("Accuracy", accuracy, 0, 1.0f);
            _stats.Skills.Add("Accuracy", skill);
            return this;
        }
        public StatsBuilder Evasion(float evasion)
        {
            var skill = new Skill("Evasion", evasion, 0, 1.0f);
            _stats.Skills.Add("Evasion", skill);
            return this;
        }
    }
}