using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PokeBot.Controllers;
using PokeBot.Dtos;
using PokeBot.Services.Helpers;

namespace PokeBot.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        public UserController _controller { get; set; }
        public DiscordSocketClient _discord { get; set; }
        public IServiceProvider _provider { get; set; }
        public InfoModule(IServiceProvider provider, DiscordSocketClient discord, UserController controller)
        {
            _discord = discord;
            _controller = controller;
            _provider = provider;
        }
        [Command("catch")]
        public async Task Catch()
        {
            var cwp = _provider.GetRequiredService<CurrentWanderingPokemon>();
            if(cwp._isCaptured)
            {
                await ReplyAsync("There aren't any pokemon around right now. Watch out for the next one!");
                return;
            }

            var user = Context.Message.Author;
            if(!(await _controller.UserExists(user.Id)))
            {
                await _controller.RegisterUser(user.Id, user.Username);
                System.Console.WriteLine($"Registered user {user.Username}");
            }
            
            PokemonForCreationDto pokemonForCreation = new PokemonForCreationDto(cwp._pokemon.PokeId, cwp._pokemon.Name);
            cwp.SetIsCaptured(true);
            await _controller.AddToUserPokeCollectionByDiscordId(user.Id, pokemonForCreation);
            
            await ReplyAsync($":medal: Congratulations, {user.Username}! You've successfully caught a `{cwp._pokemon.Name}`.\n\nType `!inventory` to see it in your inventory!");
        }

        [Command("inventory")]
        public async Task Inventory()
        {
            var user = Context.Message.Author;
            if(!(await _controller.UserExists(user.Id)))
            {
                await _controller.RegisterUser(user.Id, user.Username);
                System.Console.WriteLine($"Registered user {user.Username}");
                await ReplyAsync($"Your inventory is empty. Go catch some pokemon!");
                return;
            }

            var userData = await _controller.GetUserByDiscordId(user.Id);
            Dictionary<string, int> inventory = new Dictionary<string, int>();
            foreach(var pokemon in userData.PokeCollection)
            {
                if(!inventory.ContainsKey(pokemon.Name))
                {
                    inventory.Add(pokemon.Name, 1);
                }
                else
                {
                    inventory[pokemon.Name] += 1;
                }
            }
            var message = $":globe_with_meridians:` HELLO, {user.Username.ToUpper()}. WELCOME TO YOUR POKEMON STORAGE UNIT.\n\nBELOW YOU CAN FIND A LIST OF ALL THE POKEMON YOU'VE CAUGHT...`\n\n";
            message += $":ballot_box:`POKE STORAGE`\n";
            message += "```";
            foreach(var record in inventory)
            {
                message += $"{record.Key} | x{record.Value} \n";
            }
            message += "```";

            await ReplyAsync(message);
        }
    }
}