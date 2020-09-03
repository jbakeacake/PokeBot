namespace PokeBot.Models
{
    public class PokeType
    {
        public int Id { get; set; }
        public int PokeTypeId { get; set; }
        public string Name { get; set; }
        public string Delimited_Double_Damage_From { get; set; }
        public string Delimited_Double_Damage_To { get; set; }
        public string Delimited_Half_Damage_From { get; set; }
        public string Delimited_Half_Damage_To { get; set; }
    }
}