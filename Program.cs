using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokeBot.Controllers;
using PokeBot.Data;
using PokeBot.Services;
using PokeBot.Services.Helpers;

namespace Pokebot
{
    class Program
    {
        static void Main(string[] args)
         => new Program().MainAsync(args).GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private IConfiguration _config;

        public async Task MainAsync(string[] args)
        {
            _client = new DiscordSocketClient();
            _config = BuildConfig();

            var services = ConfigureServices();
            
            using (var scope = services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var context = serviceProvider.GetRequiredService<DataContext>();
                    context.Database.Migrate();
                    await context.Pokemon_Tbl.ForEachAsync(x => {
                        if(String.IsNullOrEmpty(x.Url))
                        {
                            x.Url = $"https://pokeres.bastionbot.org/images/pokemon/{x.PokeId}.png";
                            System.Console.WriteLine($"Updated to {x.Url}");
                        }
                    });
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                }
            }

            services.GetRequiredService<LogService>();
            services.GetRequiredService<PokemonHandlingService>().Initialize(services);
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);
            await _client.LoginAsync(TokenType.Bot, _config.GetSection("AppSettings:Token").Value);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
            => new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<PokeCache>()
                .AddSingleton<CurrentWanderingPokemon>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<PokemonHandlingService>()
                .AddSingleton(_config)
                .AddAutoMapper(typeof(PokeBotRepository).Assembly)
                .AddLogging()
                .AddSingleton<LogService>()
                .AddDbContext<DataContext>(x => x.UseSqlite(_config.GetConnectionString("DefaultConnection")))
                .AddScoped<IPokeBotRepository, PokeBotRepository>()
                .AddScoped<UserController>()
                .AddScoped<PokemonController>()
                .BuildServiceProvider();

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }
    }
}
