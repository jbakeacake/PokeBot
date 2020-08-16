using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PokeBot.Models;

namespace PokeBot.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
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
            return await _context.Users_Tbl.AnyAsync(x => x.DiscordId == discordId);
        }
    }
}