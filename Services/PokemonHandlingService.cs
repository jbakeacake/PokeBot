using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
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
        private IServiceProvider _provider;
        private PokemonController _pokeController;
        private UserController _userController;
        private System.Threading.Timer _timer;
        private readonly int MIN_MINUTE_INTERVAL = 45;
        private readonly int MAX_MINUTE_INTERVAL = 120;

        public PokemonHandlingService(DiscordSocketClient discord, IServiceProvider provider)
        {
            _discord = discord;
            _provider = provider;
            _pokeController = provider.GetRequiredService<PokemonController>();
            _userController = provider.GetRequiredService<UserController>();

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
            Console.WriteLine($"Minutes until appearance: " + minutesUntilAppearance );
            int dueTime = 1000 * 60 * minutesUntilAppearance;
            int period = 1000 * 60 * minutesUntilAppearance;
            _timer = new System.Threading.Timer(DisplayPokemon, null, dueTime, period);
            await Task.CompletedTask;
        }

        private async void DisplayPokemon(object state)
        {
            Console.WriteLine("Logging Pokemon...");
            var guild = GetRandomGuild();
            var channels = guild.Channels;
            var channel = channels.OrderBy(x => x.Id).First() as ISocketMessageChannel;
            // var channel = _discord.GetChannel(524462279689895936) as ISocketMessageChannel;

            var pokemon = await _pokeController.GetRandomPokemon();
            var cwp = _provider.GetRequiredService<CurrentWanderingPokemon>();
            cwp.SetPokemon(pokemon);
            cwp.SetIsCaptured(false);

            Embed embeddedMessage = CreateEmbeddedMessage(pokemon);

            await channel.SendMessageAsync(embed: embeddedMessage);
        }

        private Embed CreateEmbeddedMessage(PokemonForReturnDto pokemon)
        {
            var upperRule = " ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬ ";
            var horizontalRule = "-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-";

            var embeddedMessage = new EmbedBuilder()
                .WithAuthor(_discord.CurrentUser)
                .WithTitle("A Pokemon Wanders Through this Channel...")
                .WithDescription($"{upperRule} \n A wild\t `{pokemon.Name}`\t appears! \n {horizontalRule} \n\n Type `!catch` to capture it!")
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