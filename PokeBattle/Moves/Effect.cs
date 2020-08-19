namespace PokeBot.PokeBattle.Moves
{
    public class Effect : Move
    {
        public int EffectChance { get; set; }
        public string StatName { get; set; }
        public int StatChange { get; set; } // moves StageMultiplier for a skill
    }
}