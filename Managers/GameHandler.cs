using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using PokeBot.Controllers;
using PokeBot.Dtos;
using PokeBot.PokeBattle;
using PokeBot.PokeBattle.Entities;
using PokeBot.Utils;

namespace PokeBot.Managers
{
    public class GameHandler : Handler
    {
        public Dictionary<Guid, PokeBattleGame> _games { get; set; }
        private Dictionary<ulong, InvitationGroup> _pendingInvitations { get; set; } // Key: Message Id, val: 2 user Ids
        public HashSet<ulong> _pendingPlayers { get; set; }
        public HashSet<ulong> _battlingPlayers { get; set; }
        public PokemonController _pokemonController { get; set; }
        public UserController _userController { get; set; }
        public GameHandler(DiscordSocketClient discord,
            PokemonController pokemonController,
            UserController userController) : base(discord)
        {
            _pokemonController = pokemonController;
            _userController = userController;

            _games = new Dictionary<Guid, PokeBattleGame>();
            _pendingInvitations = new Dictionary<ulong, InvitationGroup>();
            _pendingPlayers = new HashSet<ulong>();
            _battlingPlayers = new HashSet<ulong>();
        }

        public async Task UpdateUserPokemon(PokeEntity pokemon)
        {
            PokemonForUpdateDto pokemonForUpdate = new PokemonForUpdateDto
            {
                Name = pokemon.Name,
                MaxHP = pokemon.Stats.MaxHP,
                Level = pokemon.Stats.Level,
                Experience = pokemon.Stats.Experience,
                Attack = pokemon.Stats.Skills["attack"].Value,
                Defense = pokemon.Stats.Skills["defense"].Value,
                SpecialAttack = pokemon.Stats.Skills["special-attack"].Value,
                SpecialDefense = pokemon.Stats.Skills["special-defense"].Value,
                Speed = pokemon.Stats.Skills["speed"].Value
            };
            await _pokemonController.UpdatePokemon(pokemon.Id, pokemonForUpdate);
        }

        public async Task NotifyPlayerOfTurn(ulong discordId)
        {
            var message = await _discord.GetUser(discordId).SendMessageAsync("YOUR TURN");
            var keys = MoveSetEmojiMapUtil.GetMoveSetEmojiMap.Keys.ToArray();
            foreach (var key in keys)
            {
                await message.AddReactionAsync(key).ConfigureAwait(false);
            }
        }

        public async Task CreateGame(ulong invitationId)
        {
            Guid battleTokenId = Guid.NewGuid();
            InvitationGroup invitation = _pendingInvitations[invitationId];
            // Update our users in our database:
            await UpdateUser(battleTokenId, invitation.PlayerOneId);
            await UpdateUser(battleTokenId, invitation.PlayerTwoId);

            //Create the game for our dictionary:
            PokeBattleGame gameToAdd = new PokeBattleGame(battleTokenId, _pokemonController, _userController);
            gameToAdd.PlayerOne = new BattlePlayer(invitation.PlayerOneId);
            gameToAdd.PlayerTwo = new BattlePlayer(invitation.PlayerTwoId);
            _games.Add(battleTokenId, gameToAdd);
        }

        public async Task RemoveGame(Guid battleTokenId)
        {
            var game = _games[battleTokenId];
            _pendingPlayers.Remove(game.PlayerOne.DiscordId);
            _pendingPlayers.Remove(game.PlayerTwo.DiscordId);
            _battlingPlayers.Remove(game.PlayerOne.DiscordId);
            _battlingPlayers.Remove(game.PlayerTwo.DiscordId);
            _games.Remove(battleTokenId);
            await _pokemonController.RemovePokeBattle(battleTokenId);
        }

        public async Task SendInviteToPlayer(SocketUser sender, SocketUser receiver)
        {
            if (_pendingPlayers.Contains(sender.Id))
            {
                _pendingPlayers.ToList().ForEach(x => System.Console.WriteLine(x));
                var busyMsg = @"You are currently busy right now. You can cancel your current invitation or game by typing `!cancel`. **Beware**, if you leave the middle of a game, your pokemon will lose experience.";
                await SendPlayerMessage(busyMsg, sender.Id);
                return;
            }
            else if (_pendingPlayers.Contains(receiver.Id))
            {
                var busyMsg = $"**{receiver.Username}** is currently busy at the moment. Try again later!";
                await SendPlayerMessage(busyMsg, sender.Id);
                return;
            }
            //Send the invitation:
            var message = EmbeddedMessageUtil.CreateInvitationMessage(_discord.CurrentUser, sender, receiver);
            var invitation = await SendPlayerMessage(message, receiver.Id);
            await invitation.AddReactionAsync(new Emoji("✅"));
            await invitation.AddReactionAsync(new Emoji("❌"));
            //Create an invitation group:
            InvitationGroup invGroup = new InvitationGroup
            {
                PlayerOneId = sender.Id,
                PlayerTwoId = receiver.Id
            };

            _pendingInvitations.Add(invitation.Id, invGroup);
            _pendingPlayers.Add(sender.Id);
            _pendingPlayers.Add(receiver.Id);
        }

        public async Task SendPokemonListToPlayer(ulong discordId)
        {
            var userForReturn = await _userController.GetUserByDiscordId(discordId);
            var listOfCollections = PokeCollectionUtil.SlicePokeCollection((List<PokemonForReturnDto>)userForReturn.PokeCollection, 20);
            string message = "```🍚 Type `!choose #` where '#' is the pokemon's corresponding Id number.```";
            foreach (var collection in listOfCollections)
            {
                message += PokeCollectionUtil.ChunkToString(collection);
                await SendPlayerMessage(message, discordId);
                message = "";
            }
        }

        public async Task SendOpponentWaitingOnPlayer(ulong discordId, PokemonForReturnDto chosenPokemon, string thumbnailUrl)
        {
            var waitingMessage = EmbeddedMessageUtil.CreateWaitingForOtherPlayerEmbed(_discord.CurrentUser, chosenPokemon, thumbnailUrl);
            await SendPlayerMessage(waitingMessage, discordId);
        }

        public async Task SendPlayerAwaitingOnOther(ulong discordId)
        {
            var message = EmbeddedMessageUtil.CreateAwaitingOpponentEmbed(_discord.CurrentUser);
            await SendPlayerMessage(message, discordId);
        }

        public async Task SendWinnerCard(BattlePlayer player)
        {
            var message = EmbeddedMessageUtil.CreateWinningPlayerEmbed(_discord.CurrentUser, player);
            await SendPlayerMessage(message, player.DiscordId);
        }

        public async Task SendLoserCard(BattlePlayer player)
        {
            var message = EmbeddedMessageUtil.CreateLosingPlayerEmbed(_discord.CurrentUser, player);
            await SendPlayerMessage(message, player.DiscordId);
        }

        public async Task SendPlayersBattleScene(PokeBattleGame game)
        {
            var playerOneScene = await EmbeddedMessageUtil.CreateBattleSceneEmbed(_discord.CurrentUser, game.PlayerOne, game.PlayerTwo, _pokemonController);
            var playerTwoScene = await EmbeddedMessageUtil.CreateBattleSceneEmbed(_discord.CurrentUser, game.PlayerTwo, game.PlayerOne, _pokemonController);

            await SendPlayerMessage(playerOneScene, game.PlayerOne.DiscordId);
            await SendPlayerMessage(playerTwoScene, game.PlayerTwo.DiscordId);
        }
        public async Task SendPlayersLogMessage(PokeBattleGame game)
        {
            var message = "```\n";
            message += game.LogMessage;
            message += "```";

            await SendPlayerMessage(message, game.PlayerOne.DiscordId);
            await SendPlayerMessage(message, game.PlayerTwo.DiscordId);
        }
        public bool AreBothPlayersReady(Guid battleTokenId)
        {
            if (!_games.ContainsKey(battleTokenId)) return false;

            return _games[battleTokenId].isGameReady();
        }

        public bool isPlayerBusy(ulong discordId)
        {
            return _battlingPlayers.Contains(discordId);
        }
        public (ulong, ulong) GetPendingPlayersFromInvitationGroup(ulong invitationId)
        {
            if (!_pendingInvitations.ContainsKey(invitationId)) return (0, 0);
            var invitationGroup = _pendingInvitations[invitationId];
            return (invitationGroup.PlayerOneId, invitationGroup.PlayerTwoId);
        }
        private async Task UpdateUser(Guid battleTokenId, ulong discordId)
        {
            UserForUpdateDto userForUpdate = new UserForUpdateDto(battleTokenId);
            await _userController.UpdateUser(discordId, userForUpdate);
        }

        public void RemovePendingInvite(ulong invitationId)
        {
            var invitation = _pendingInvitations[invitationId];
            RemovePlayerFromPendingSet(invitation.PlayerOneId);
            RemovePlayerFromPendingSet(invitation.PlayerTwoId);
            _pendingInvitations.Remove(invitationId);
        }

        private void RemovePlayerFromPendingSet(ulong discordId)
        {
            if (!_pendingPlayers.Contains(discordId)) return;

            _pendingPlayers.Remove(discordId);
        }

        public (ulong, ulong) GetInvitiationGroupByPlayerId(ulong discordId)
        {
            var record = _pendingInvitations.FirstOrDefault(x => x.Value.PlayerOneId == discordId || x.Value.PlayerTwoId == discordId);
            return (record.Value.PlayerOneId, record.Value.PlayerTwoId);
        }

        struct InvitationGroup
        {
            public ulong PlayerOneId { get; set; }
            public ulong PlayerTwoId { get; set; }
        }
    }
}