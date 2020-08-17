using System.Collections.Generic;
using System.Threading.Tasks;
using PokeBot.Models;

namespace PokeBot.Data
{
    public interface IPokeBotRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<User> GetUser(int id);
        Task<Pokemon> GetPokemon(int pokeId);
        Task<Pokemon> GetPokemonByPokeId(int pokeId);
        Task<User> GetUserByDiscordId(ulong discordId);
        Task<IEnumerable<User>> GetUsers();
        Task<bool> UserExists(ulong discordId);
        Task<bool> PokemonExists(int pokeId);
    }
}