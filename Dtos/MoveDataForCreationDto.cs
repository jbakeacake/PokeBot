namespace PokeBot.Dtos
{
    public class MoveDataForCreationDto
    {
        public MoveDataForCreationDto()
        {
        }

        public MoveDataForCreationDto(string name, int moveId, float accuracy, float effect_Chance, string ailmentName, float power, int pP, string type, string statChangeName, int statChangeValue, string target)
        {
            this.Name = name;
            this.MoveId = moveId;
            this.Accuracy = accuracy;
            this.Effect_Chance = effect_Chance;
            this.AilmentName = ailmentName;
            this.Power = power;
            this.PP = pP;
            this.Type = type;
            this.StatChangeName = statChangeName;
            this.StatChangeValue = statChangeValue;
            this.Target = target;
        }
        public override string ToString()
        {
            var message = $"{Name},\n{MoveId},\n{Accuracy},\n{Effect_Chance},\n{AilmentName},\n{Power},\n{PP},\n{Type},\n{StatChangeName},\n{StatChangeValue},\n{Target}\n";
            return message;
        }
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