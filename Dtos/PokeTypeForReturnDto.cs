namespace PokeBot.Dtos
{
    public class PokeTypeForReturnDto
    {
        public int PokeTypeId { get; set; }
        public string Name { get; set; }
        public string[] Double_Damage_From { get; set; }
        public string[] Double_Damage_To { get; set; }
        public string[] Half_Damage_From { get; set; }
        public string[] Half_Damage_To { get; set; }
    }
}