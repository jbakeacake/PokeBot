namespace PokeBot.Dtos
{
    public class UserForCreationDto
    {
        public ulong DiscordId { get; set; }
        public string Name { get; set; }

        public UserForCreationDto(ulong discordId, string name)
        {
            DiscordId = discordId;
            Name = name;
        }
    }
}