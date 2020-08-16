﻿using System;
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
                .AddSingleton<CurrentWanderingPokemon>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<PokemonHandlingService>()
                .AddSingleton(_config)
                .AddAutoMapper(typeof(UserRepository).Assembly)
                .AddLogging()
                .AddSingleton<LogService>()
                .AddDbContext<DataContext>(x => x.UseSqlite(_config.GetConnectionString("DefaultConnection")))
                .AddScoped<IUserRepository, UserRepository>()
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
