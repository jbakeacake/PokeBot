namespace PokeBot.PokeBattle.Common
{
    public class Skill
    {
        public string Name { get; set; }
        public float Value { get; set; }
        public int Stage { get; set; }
        public float StageMultiplier { get; set; }
        public Skill(string name, float value, int stage, float stageMultiplier)
        {
            Name = name;
            Value = value;
            Stage = stage;
            StageMultiplier = stageMultiplier;
        }

        public void CalculateStageMultiplier(int statChange)
        {
            var updatedStage = Stage + statChange;
            
            if(updatedStage < -6)
            {
                Stage = -6;
            }
            else if (updatedStage > 6)
            {
                Stage = 6;
            }
            else
            {
                Stage = updatedStage;
            }

            StageMultiplier = StageMultiplierUtils.GetBasicStatsMap[Stage];
        }
    }
}