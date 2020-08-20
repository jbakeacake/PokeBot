using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PokeBot.Controllers;
using PokeBot.PokeBattle;

namespace PokeBot.Modules
{
    public class BattleModule : ModuleBase<SocketCommandContext>
    {
        public Dictionary<Guid, PokeBattleStateManager> GameRecords { get; set; }
        public UserController _userController { get; set; }
        public PokemonController _pokemonController { get; set; }
        private readonly IServiceProvider _provider;
        public BattleModule(IServiceProvider provider)
        {
            _provider = provider;
            _userController = provider.GetRequiredService<UserController>();
            _pokemonController = provider.GetRequiredService<PokemonController>();
        }

        [Command("duel")]
        public async Task Duel(IUser receiver)
        {

            IUser sender = Context.Message.Author;
            System.Console.WriteLine("Sending invite...");
            if(await isBusy(receiver))
            {
                await sender.SendMessageAsync("User is currently busy with another duel! Try again later.");
                return;
            }

            
        }

        private async Task<bool> isBusy(IUser receiver)
        {
            var user = await _userController.GetUserByDiscordId(receiver.Id);
            return user.BattleTokenId != Guid.Empty;
        }
    }
}