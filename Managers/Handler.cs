using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace PokeBot.Managers
{
    public class Handler : IHandler
    {
        protected readonly DiscordSocketClient _discord;
        public Handler(DiscordSocketClient discord)
        {
            _discord = discord;
        }
        public async Task<IUserMessage> SendPlayerMessage(string content, ulong discordId)
        {
            var message = await _discord.GetUser(discordId).SendMessageAsync(content);
            return message;
        }

        public async Task<IUserMessage> SendPlayerMessage(Embed embeddedContent, ulong discordId)
        {
            var message = await _discord.GetUser(discordId).SendMessageAsync(embed: embeddedContent);
            return message;
        }
    }
}