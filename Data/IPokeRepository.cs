using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokeBot.Models;

namespace PokeBot.Data
{
    public interface IPokeRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<User> GetUser(int id);
        Task<Pokemon> GetPokemon(int id);
        Task<PokemonData> GetPokemonData(int id);
        Task<PokeBattleData> GetPokeBattleData(int id);
        Task<MoveData> GetMoveData(int id);
        Task<MoveData> GetMoveDataByMoveId(int moveId);
        Task<Pokemon> GetPokemonByPokeId(int pokeId);
        Task<User> GetUserByDiscordId(ulong discordId);
        Task<PokeBattleData> GetPokeBattleDataByGUID(Guid battleId);
        Task<PokemonData> GetPokemonDataByPokeId(int pokeId);
        Task<IEnumerable<User>> GetUsers();
        Task<IEnumerable<MoveData>> GetMoves();
        Task<IEnumerable<PokemonData>> GetPokemonDatas();
        Task<IEnumerable<PokeBattleData>> GetPokeBattles();
        Task<bool> UserExists(ulong discordId);
        Task<bool> PokemonExists(int pokeId);
        Task<bool> MoveExists(int moveId);
    }
}