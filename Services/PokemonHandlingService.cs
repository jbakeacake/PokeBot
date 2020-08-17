using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokeBot.Controllers;
using PokeBot.Dtos;
using PokeBot.Models;
using PokeBot.Services.Helpers;

namespace PokeBot.Services
{
    public class PokemonHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly PokeCache _cache;
        private IServiceProvider _provider;
        private PokemonController _pokeController;
        private UserController _userController;
        private System.Threading.Timer _timer;
        private readonly int MIN_MINUTE_INTERVAL = 1;
        private readonly int MAX_MINUTE_INTERVAL = 40;
        private int _period;
        private ulong _channelId;

        public PokemonHandlingService(DiscordSocketClient discord, IServiceProvider provider, IConfiguration config)
        {
            _discord = discord;
            _provider = provider;
            _cache = provider.GetRequiredService<PokeCache>();
            _pokeController = provider.GetRequiredService<PokemonController>();
            _userController = provider.GetRequiredService<UserController>();
            _channelId = config.GetValue<ulong>("AppSettings:ChannelId");

            _discord.Ready += SendPokemonAppearance;
        }

        public void Initialize(IServiceProvider provider)
        {
            _provider = provider;
        }

        private async Task SendPokemonAppearance()
        {
            Random rand = new Random();
            var minutesUntilAppearance = rand.Next(MIN_MINUTE_INTERVAL, MAX_MINUTE_INTERVAL);
            int dueTime = 1000 * 60 * 10; // initial 10 minutes
            _period = 1000 * 60 * minutesUntilAppearance;
            _timer = new System.Threading.Timer(DisplayPokemon, null, dueTime, _period);
            await Task.CompletedTask;
        }

        private async void DisplayPokemon(object state)
        {
            Random rand = new Random();
            var newMinutes = rand.Next(MIN_MINUTE_INTERVAL, MAX_MINUTE_INTERVAL + 1);
            _period = 1000 * 60 * newMinutes;
            Console.WriteLine($"Logging next Pokemon in {newMinutes}...");

            var channel = _discord.GetChannel(_channelId) as ISocketMessageChannel;

            var pokemon = await _cache.GetPokemon();
            var cwp = _provider.GetRequiredService<CurrentWanderingPokemon>();
            cwp.SetPokemon(pokemon);
            cwp.SetIsCaptured(false);

            Embed embeddedMessage = CreateEmbeddedMessage(pokemon);

            await channel.SendMessageAsync(embed: embeddedMessage);
        }

        private Embed CreateEmbeddedMessage(PokemonForReturnDto pokemon)
        {
            var upperRule = "╔═════════════════";
            var lowerRule = "╚═════════════════";

            var embeddedMessage = new EmbedBuilder()
                .WithAuthor(_discord.CurrentUser)
                .WithTitle("A Pokemon Wanders Through this Channel...")
                .WithDescription($"{upperRule} \n║ A wild `{pokemon.Name}` appears!\n{lowerRule} \n\n Type `!catch` to capture it!")
                .WithImageUrl(pokemon.Url)
                .WithFooter(footer => footer.Text = "Appeared ")
                .WithCurrentTimestamp()
                .Build();

            return embeddedMessage;
        }

        private SocketGuild GetRandomGuild()
        {
            var guilds = _discord.Guilds;
            Random rand = new Random();
            var skips = rand.Next(guilds.Count);
            var guild = guilds.Skip(skips).Take(1).First();

            return guild;
        }
    }
}