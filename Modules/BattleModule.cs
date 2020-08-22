using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PokeBot.Controllers;
using PokeBot.Dtos;
using PokeBot.Modules.Helpers;
using PokeBot.PokeBattle;
using PokeBot.Services;

namespace PokeBot.Modules
{
    public class BattleModule : ModuleBase<SocketCommandContext>
    {
        public UserController _userController { get; set; }
        public PokemonController _pokemonController { get; set; }
        private readonly IServiceProvider _provider;
        private readonly PokeBattleHandlingService _pokeBattleService;
        public BattleModule(IServiceProvider provider, UserController userController, PokemonController pokemonController, PokeBattleHandlingService pokeBattleHandlingService)
        {
            _provider = provider;
            _userController = userController;
            _pokemonController = pokemonController;
            _pokeBattleService = pokeBattleHandlingService;
        }

        [Command("duel")]
        public async Task Duel(SocketUser receiver)
        {

            SocketUser sender = Context.Message.Author;
            System.Console.WriteLine("Sending invite...");
            if(await isBusy(receiver))
            {
                await sender.SendMessageAsync("User is currently busy with another duel! Try again later.");
                return;
            }
            //Create Invitation Message:
            var embeddedMessage = EmbeddedMessageHelper.CreateInvitationMessage(Context.Client.CurrentUser, sender, receiver);
            var invite = await receiver.SendMessageAsync(embed: embeddedMessage);
            await invite.AddReactionAsync(new Emoji("✅"));
            await invite.AddReactionAsync(new Emoji("❌"));

            _pokeBattleService._busyUsers.Add(sender.Id);
            _pokeBattleService._busyUsers.Add(receiver.Id);
            _pokeBattleService._pendingInvites.Add(invite.Id, (sender.Id, receiver.Id));
        }

        [Command("choose")]
        public async Task Choose(int pokemonId)
        {
            var user = Context.Message.Author;
            var battleTokenId = (await _userController.GetUserByDiscordId(user.Id)).BattleTokenId;
            
            //Ignore if the user isn't busy OR if they dont have a game setup 
            if(!(await isBusy(user))
                || !_pokeBattleService._currentGames.ContainsKey(battleTokenId)) return;

            //Initialize this player's pokemon for their game
            await _pokeBattleService.InitializePlayerAsync(battleTokenId, user.Id, pokemonId);

            var chosenPokemon = await _pokemonController.GetPokemonFromUserInventory(pokemonId);
            var waitingMessage = CreateWaitingForOtherPlayerEmbed(chosenPokemon);
            await user.SendMessageAsync(embed: waitingMessage);

            var game = _pokeBattleService._currentGames[battleTokenId];
            if(areBothPlayersReady(game))
            {
                System.Console.WriteLine("BINGO");
                await game.InitializeAsync(battleTokenId);
                game.StartGame();
            }
        }

        private bool areBothPlayersReady(PokeBattleGame game)
        {
            return game.PlayerOne.CurrentPokemon != null && game.PlayerTwo.CurrentPokemon != null; 
        }

        private Embed CreateWaitingForOtherPlayerEmbed(PokemonForReturnDto chosenPokemon)
        {
            return new EmbedBuilder()
                .WithAuthor(Context.Client.CurrentUser)
                .WithTitle("Awaiting other player's choice…")
                .WithDescription($"Your Chosen Pokemon ► {chosenPokemon.Name} • {chosenPokemon.Level} • {chosenPokemon.MaxHP} / {chosenPokemon.MaxHP}")
                .WithFooter(footer => footer.Text = "Waiting Since ")
                .WithCurrentTimestamp()
                .Build();
        }

        private async Task<bool> isBusy(IUser user)
        {
            var userForReturn = await _userController.GetUserByDiscordId(user.Id);

            return userForReturn == null 
                || _pokeBattleService._busyUsers.Contains(user.Id)
                || userForReturn.BattleTokenId != Guid.Empty;
        }
    }
}