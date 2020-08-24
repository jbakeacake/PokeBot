using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PokeBot.Dtos;
using PokeBot.Models;

namespace PokeBot.Data
{
    public class PokeRepository : IPokeRepository
    {
        private readonly DataContext _context;
        public PokeRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users_Tbl.Include(x => x.PokeCollection).FirstOrDefaultAsync(x => x.Id == id);
            return user;
        }

        public async Task<User> GetUserByDiscordId(ulong discordId)
        {
            var user = await _context.Users_Tbl.Include(x => x.PokeCollection).FirstOrDefaultAsync(x => x.DiscordId == discordId);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users_Tbl.Include(x => x.PokeCollection).ToListAsync();
            return users;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UserExists(ulong discordId)
        {
            return await _context.Users_Tbl.AsQueryable().AnyAsync(x => x.DiscordId == discordId);
        }

        public async Task<bool> PokemonExists(int pokeId)
        {
            return await _context.Pokemon_Tbl.AsQueryable().AnyAsync(x => x.PokeId == pokeId);
        }

        public Task<PokemonData> GetPokemonData(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PokeBattleData> GetPokeBattleData(int id)
        {
            throw new NotImplementedException();
        }

        public Task<MoveData> GetMoveData(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<MoveData> GetMoveDataByMoveId(int moveId)
        {
            var moveData = await _context.MoveData_Tbl.AsQueryable().FirstOrDefaultAsync(x => x.MoveId == moveId);
            return moveData;
        }

        public Task<PokeBattleData> GetPokeBattleDataByGUID(Guid battleId)
        {
            throw new NotImplementedException();
        }

        public async Task<PokemonData> GetPokemonDataByPokeId(int pokeId)
        {
            var pokemonData = await _context.PokeData_Tbl.AsQueryable().FirstOrDefaultAsync(x => x.PokeId == pokeId);
            return pokemonData;
        }

        public Task<IEnumerable<MoveData>> GetMoves()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PokemonData>> GetPokemonDatas()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PokeBattleData>> GetPokeBattles()
        {
            throw new NotImplementedException();
        }

        public Task<bool> MoveExists(int moveId)
        {
            throw new NotImplementedException();
        }

        public Task<Pokemon> GetPokemon(int id)
        {
            var pokemon = _context.Pokemon_Tbl.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);
            return pokemon;
        }

        public Task<Pokemon> GetPokemonByPokeId(int pokeId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetUsersByBattleTokenId(Guid battleTokenId)
        {
            var users = await _context.Users_Tbl.AsQueryable().Where(x => x.BattleTokenId.Equals(battleTokenId)).ToListAsync();
            return users;
        }

        public async Task<PokeType> GetPokeTypeByName(string name)
        {
            var pokeType = await _context.PokeType_Tbl.AsQueryable().FirstOrDefaultAsync(x => x.Name == name);
            return pokeType;
        }
    }
}