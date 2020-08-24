using System.Threading.Tasks;
using Discord;

namespace PokeBot.Managers
{
    public interface IHandler
    {
        Task<IUserMessage> SendPlayerMessage(string content, ulong discordId);
        Task<IUserMessage> SendPlayerMessage(Embed embeddedContent, ulong discordId);
    }
}