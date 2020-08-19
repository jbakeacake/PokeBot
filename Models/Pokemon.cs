namespace PokeBot.Models
{
    public class Pokemon
    {
        public int Id { get; set; }
        public int PokeId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        
    }
}