using System.Collections.Generic;
using System.Threading.Tasks;
using PokeBot.Models;

namespace PokeBot.Data
{
    public interface IUserRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<User> GetUser(int id);
        Task<User> GetUserByDiscordId(ulong discordId);
        Task<IEnumerable<User>> GetUsers();
        Task<bool> UserExists(ulong discordId);
    }
}