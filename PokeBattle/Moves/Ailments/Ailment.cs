namespace PokeBot.PokeBattle.Moves.Ailments
{
    public class Ailment
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int AilmentChance { get; set; }
        public Ailment(string name, string description, int ailmentChance)
        {
            Name = name;
            Description = description;
            AilmentChance = ailmentChance;
        }
    }
}