using PokeBot.PokeBattle.Moves.Ailments;

namespace PokeBot.PokeBattle.Moves
{
    public class Move : IMove
    {
        public string Name { get; set; }
        public int StageMultiplierForAccuracy { get; set; } // always starts at 1
        public float Accuracy { get; set; }
        public float EffectChance { get; set; } // also designated ailment chance
        public Ailment Ailment { get; set; }
        public float Power { get; set; }
        public int PP { get; set; }
        public string Type { get; set; }
        public string StatChangeName { get; set; }
        public int StatChangeValue { get; set; }
        public bool TargetsOther { get; set; }
        public Move() {}
    }
}