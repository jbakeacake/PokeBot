namespace PokeBot.Dtos
{
    public class MoveDataForReturnDto
    {
        public int MoveId { get; set; }
        public string Name { get; set; }
        public float Accuracy { get; set; }
        public float Effect_Chance { get; set; }
        public string AilmentName { get; set; }
        public float Power { get; set; }
        public int PP { get; set; }
        public string Type { get; set; }
        public string StatChangeName { get; set; }
        public int StatChangeValue { get; set; }
        public string Target { get; set; } // Unless this field is "user" it targets another pokemon
    }
}