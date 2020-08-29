using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PokeBot.Controllers;
using PokeBot.Dtos;
using PokeBot.Managers;
using PokeBot.PokeBattle;
using PokeBot.Utils;

namespace PokeBot.Services
{
    public class PokeBattleHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private IServiceProvider _provider;
        private PokemonController _pokeController;
        private UserController _userController;
        public GameHandler _handler { get; set; }
        public PokeBattleHandlingService(DiscordSocketClient discord, IServiceProvider provider, PokemonController pokemonController, UserController userController)
        {
            _discord = discord;
            _provider = provider;
            _pokeController = pokemonController;
            _userController = userController;

            _handler = new GameHandler(discord, pokemonController, userController);

            _discord.ReactionAdded += ListenForInviteReactions;
            _discord.ReactionAdded += ListenForBattleReactions;
        }

        public void Initialize(IServiceProvider provider)
        {
            _provider = provider;
        }

        private async Task ListenForInviteReactions(Cacheable<IUserMessage, ulong> before, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var message = await before.GetOrDownloadAsync();
            if (message == null) return;
            //If the user being messaged is already busy
            if (_handler.isPlayerInBattle(reaction.UserId)) return;
            //If the message was authored by a user
            if (message.Source == MessageSource.User) return;
            //If the reaction was made by the Bot
            if (reaction.UserId == _discord.CurrentUser.Id) return;

            await DoInviteAction(reaction, message);
        }

        private async Task ListenForBattleReactions(Cacheable<IUserMessage, ulong> before, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == _discord.CurrentUser.Id) return;
            var moveSetMap = MoveSetEmojiMapUtil.GetMoveSetEmojiMap;
            if (!moveSetMap.ContainsKey(reaction.Emote)) return;

            //Get the message that was reacted to:
            var message = await before.GetOrDownloadAsync();

            //Execute the current players turn
            int chosenMove = moveSetMap[reaction.Emote];
            var user = await _userController.GetUserByDiscordId(reaction.UserId);
            
            if(!_handler._games.ContainsKey(user.BattleTokenId))
            {
                await message.DeleteAsync();
                return;
            }
            
            var game = _handler._games[user.BattleTokenId];
            var moveIndex = MoveSetEmojiMapUtil.GetMoveSetEmojiMap[reaction.Emote];
            // Check if its the user's turn:
            if (!game.isPlayersTurn(user.DiscordId)) return;
            // Run the game, executing the current players turn, thus making it the next players turn
            game.RunGame(moveIndex);

            //Now, notify that the next player's turn is ready
            var currentUser = game.GetPlayer(user.DiscordId);
            var nextPlayer = game.GetPlayerForMove();
            //If the game is over, let us know:
            if (game.isGameOver())
            {
                game.RunGame(-1); // Execute post-game functions (XP gain / Level Up)
                var winner = game.GetWinningPlayer();
                var loser = game.GetLosingPlayer();
                await _handler.UpdateUserPokemon(winner.CurrentPokemon);
                await _handler.UpdateUserPokemon(loser.CurrentPokemon);
                await _handler.SendWinnerCard(winner);
                await _handler.SendLoserCard(loser);
                await game.ClearTokens();
                await message.DeleteAsync();
                await _handler.RemoveGame(game.BattleTokenId);
                return;
            }
            // After the execution of the move, 
            // alert both players of the updated scene
            await _handler.SendPlayersBattleScene(game);
            await _handler.SendPlayersLogMessage(game);
            //Notify the next player's turn;
            await _handler.NotifyPlayerOfTurn(nextPlayer.DiscordId);
            if (message != null)
                await message.DeleteAsync();
        }

        private async Task DoInviteAction(SocketReaction reaction, IUserMessage message)
        {
            if(!_handler._pendingPlayers.Contains(reaction.UserId))
            {
                await message.DeleteAsync();
                return;
            }
            if (reaction.Emote.Name.Equals("✅"))
            {
                await _handler.CreateGame(message.Id);
                await message.DeleteAsync();
                var (playerOneId, playerTwoId) = _handler.GetPendingPlayersFromInvitationGroup(message.Id);
                await _handler.SendPokemonListToPlayer(playerOneId);
                await _handler.SendPokemonListToPlayer(playerTwoId);
                _handler.RemovePendingInvite(message.Id);
            }
            else if (reaction.Emote.Name.Equals("❌"))
            {
                _handler.RemovePendingInvite(message.Id);
                await message.DeleteAsync();
            }
        }
    }
}