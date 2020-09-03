using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PokeBot.Controllers;
using PokeBot.Dtos;
using PokeBot.Utils;
using PokeBot.PokeBattle;
using PokeBot.Services;
using PokeBot.Managers;
using PokeBot.PokeBattle.Entities;
using System.Linq;
using PokeBot.PokeBattle.Moves;
using System.Threading;

namespace PokeBot.Modules
{
    public class BattleModule : ModuleBase<SocketCommandContext>
    {
        public UserController _userController { get; set; }
        public PokemonController _pokemonController { get; set; }
        private readonly IServiceProvider _provider;
        private GameHandler _gameHandler { get; set; }
        public BattleModule(IServiceProvider provider, UserController userController, PokemonController pokemonController, PokeBattleHandlingService pokeBattleHandlingService)
        {
            _provider = provider;
            _userController = userController;
            _pokemonController = pokemonController;
            _gameHandler = pokeBattleHandlingService._handler;
        }

        [Command("cancel", RunMode = RunMode.Async)]
        public async Task CancelDuel()
        {
            var user = Context.Message.Author;
            var userFromTable = await _userController.GetUserByDiscordId(user.Id);
            var playersInvolved = _gameHandler.GetInvitiationGroupByPlayerId(user.Id);
            System.Console.WriteLine($"Player one: {playersInvolved.Item1}|Player Two: {playersInvolved.Item2}");
            //Remove if the players are pending
            RemovePlayerFromBusyLists(_gameHandler._pendingPlayers, playersInvolved.Item1);
            RemovePlayerFromBusyLists(_gameHandler._pendingPlayers, playersInvolved.Item2);
            //Remove if the players are battling
            RemovePlayerFromBusyLists(_gameHandler._battlingPlayers, playersInvolved.Item1);
            RemovePlayerFromBusyLists(_gameHandler._battlingPlayers, playersInvolved.Item2);
            //Finally, remove the game from our dictionary (if theyre in the middle of a game):
            if(_gameHandler._games.ContainsKey(userFromTable.BattleTokenId))
            {
                var game = _gameHandler._games[userFromTable.BattleTokenId];
                UserForUpdateDto userWithCleanedToken = new UserForUpdateDto(Guid.Empty);
                await _userController.UpdateUser(game.PlayerOne.DiscordId, userWithCleanedToken);
                await _userController.UpdateUser(game.PlayerTwo.DiscordId, userWithCleanedToken); 
                await _pokemonController.RemovePokeBattle(userFromTable.BattleTokenId);
                _gameHandler._games.Remove(userFromTable.BattleTokenId);
            }

            await user.SendMessageAsync("Game/Invite Cancelled.");
        }

        private void RemovePlayerFromBusyLists(HashSet<ulong> busyList, ulong discordId)
        {
            if(busyList.Contains(discordId))
            {
                System.Console.WriteLine($"{discordId} removed from busy set.");
                busyList.Remove(discordId);
            }
        }

        [Command("duel")]
        public async Task Duel(SocketUser receiver)
        {
            SocketUser sender = Context.Message.Author;
            if(receiver.IsBot) return;
            if(sender.Id == receiver.Id) return;
            
            System.Console.WriteLine("Sending invite...");
            await _gameHandler.SendInviteToPlayer(sender, receiver);
        }

        [Command("choose", RunMode = RunMode.Async)]
        public async Task Choose(int pokemonId)
        {
            var user = Context.Message.Author;
            var userFromRepo = await _userController.GetUserByDiscordId(user.Id);
            var battleTokenId = (await _userController.GetUserByDiscordId(user.Id)).BattleTokenId;

            if (!userFromRepo.PokeCollection.Any(x => x.Id == pokemonId)) return;
            if (_gameHandler.isPlayerBusy(user.Id)) return;
            if (battleTokenId == Guid.Empty) return;
            if(!_gameHandler._games.ContainsKey(battleTokenId)) return;


            var game = _gameHandler._games[battleTokenId];
            var player = game.GetPlayer(user.Id);

            var pokemon = await _pokemonController.GetPokemonFromUserInventory(pokemonId);
            var pokemonData = await _pokemonController.GetPokemonData(pokemon.PokeId);
            var pokeType = await _pokemonController.GetPokeType(pokemon.Type);
            var moves = await GetMovesFromIds(pokemon.MoveIds);
            player.InitializeCurrentPokemon(pokemon, pokeType, moves, pokemon.Id);
            System.Console.WriteLine(game.GetPlayer(user.Id).CurrentPokemon.Name + " initialized!");

            //Initialize this player's pokemon for their game
            if (game.isGameReady())
            {
                //Add these players to the "battlePlayers" set:
                _gameHandler._battlingPlayers.Add(game.PlayerOne.DiscordId);
                _gameHandler._battlingPlayers.Add(game.PlayerTwo.DiscordId);
                //Initialize the game:
                await game.InitializeAsync(battleTokenId);
                //Run the game:
                game.RunGame(-1);
                //Notify both players that the game is starting:
                var versusCard = EmbeddedMessageUtil.CreateVersusPokemonEmbed(Context.Client.CurrentUser, game.PlayerOne, game.PlayerTwo);
                var playerOneBattleCard = await EmbeddedMessageUtil.CreateBattleSceneEmbed(Context.Client.CurrentUser, game.PlayerOne, game.PlayerTwo, _pokemonController);
                var playerTwoBattleCard = await EmbeddedMessageUtil.CreateBattleSceneEmbed(Context.Client.CurrentUser, game.PlayerTwo, game.PlayerOne, _pokemonController);
                await _gameHandler.SendPlayerMessage(versusCard, game.PlayerOne.DiscordId);
                await _gameHandler.SendPlayerMessage(versusCard, game.PlayerTwo.DiscordId);
                //Send each player battle scenes:
                await _gameHandler.SendPlayersBattleScene(game);

                //Notify the player that is going first to make a move:
                var nextPlayer = game.GetPlayerForMove();
                await _gameHandler.NotifyPlayerOfTurn(nextPlayer.DiscordId);
            }
            else
            {
                await _gameHandler.SendPlayerAwaitingOnOther(user.Id);
            }
        }
        //TODO : MOVE TO HELPER OBJECT
        private async Task<Move[]> GetMovesFromIds(int[] moveIds)
        {
            Move[] arr = new Move[moveIds.Length];
            for (int i = 0; i < moveIds.Length; i++)
            {
                var moveData = await _pokemonController.GetMoveData(moveIds[i]);
                arr[i] = CreateMove(moveData);
            }

            return arr;
        }
        //TODO : MOVE TO HELPER OBJECT
        private Move CreateMove(MoveDataForReturnDto moveData)
        {
            return new MoveBuilder()
                .Name(moveData.Name)
                .StageMultiplierForAccuracy(1)
                .Accuracy(moveData.Accuracy)
                .EffectChance(moveData.Effect_Chance)
                .Ailment(moveData.AilmentName)
                .Power(moveData.Power)
                .PP(moveData.PP)
                .Type(moveData.Type)
                .StatChangeName(moveData.StatChangeName)
                .StatChangeValue(moveData.StatChangeValue)
                .TargetsOther(moveData.Target)
                .Build();
        }
    }
}