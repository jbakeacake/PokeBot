using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PokeBot.Models;

namespace PokeBot.Data
{
    public class DataContext : DbContext
    {
        public DataContext() : base() { }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            IConfiguration config = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("appsettings.json", true, true)
                                        .Build();
            builder.UseSqlite(config.GetConnectionString("DefaultConnection"));
        }

        public DbSet<User> Users_Tbl { get; set; }
        public DbSet<Pokemon> Pokemon_Tbl { get; set; }
        public DbSet<PokemonData> PokeData_Tbl { get; set; }
        public DbSet<MoveData> MoveData_Tbl { get; set; }
        public DbSet<MoveLink> MoveLink_Tbl { get; set; }
        public DbSet<PokeBattleData> PokeBattleData_Tbl { get; set; }
    }
}