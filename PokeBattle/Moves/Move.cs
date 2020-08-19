using PokeBot.PokeBattle.Moves.Ailments;

namespace PokeBot.PokeBattle.Moves
{
    public class Move : IMove
    {
        public int StageMultiplierForAccuracy { get; set; }
        public bool TargetsOther { get; set; }
        public float Accuracy { get; set; }
        public int PP { get; set; }
        public int Power { get; set; }
        public string Type { get; set; }
        public Ailment Ailment { get; set; }
        public Move() {}
    }
}