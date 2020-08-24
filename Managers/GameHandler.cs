using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using PokeBot.Controllers;
using PokeBot.Dtos;
using PokeBot.PokeBattle;
using PokeBot.Utils;

namespace PokeBot.Managers
{
    public class GameHandler : Handler
    {
        public Dictionary<Guid, PokeBattleGame> _games { get; set; }
        private Dictionary<ulong, InvitationGroup> _pendingInvitations { get; set; }
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
            _battlingPlayers = new HashSet<ulong>();
        }

        public async Task NotifyPlayerOfTurn(ulong discordId)
        {
            var message = await _discord.GetUser(discordId).SendMessageAsync("YOUR TURN");
            foreach(var key in MoveSetEmojiMapUtil.GetMoveSetEmojiMap.Keys)
            {
                await message.AddReactionAsync(key);
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
            PokeBattleGame gameToAdd = new PokeBattleGame(_pokemonController, _userController);
            gameToAdd.PlayerOne = new BattlePlayer(invitation.PlayerOneId);
            gameToAdd.PlayerTwo = new BattlePlayer(invitation.PlayerTwoId);
            _games.Add(battleTokenId, gameToAdd);
        }

        public async Task SendInviteToPlayer(SocketUser sender, SocketUser receiver)
        {
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
        }

        public async Task SendPokemonListToPlayer(ulong discordId)
        {
            var userForReturn = await _userController.GetUserByDiscordId(discordId);
            var pokeListMessage = EmbeddedMessageUtil.CreateChoosePokemonMessage(_discord.CurrentUser, userForReturn);
            await SendPlayerMessage(pokeListMessage, discordId);
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

        public bool AreBothPlayersReady(Guid battleTokenId)
        {
            if(!_games.ContainsKey(battleTokenId)) return false;

            return _games[battleTokenId].isGameReady();
        }

        public bool isPlayerInBattle(ulong discordId)
        {
            return _battlingPlayers.Contains(discordId);
        }
        public (ulong, ulong) GetPendingPlayersFromInvitationGroup(ulong invitationId)
        {
            if(!_pendingInvitations.ContainsKey(invitationId)) return (0,0);
            var invitationGroup = _pendingInvitations[invitationId];
            return (invitationGroup.PlayerOneId, invitationGroup.PlayerTwoId);
        }
        private async Task UpdateUser(Guid battleTokenId, ulong discordId)
        {
            UserForUpdateDto userForUpdate = new UserForUpdateDto(battleTokenId);
            await _userController.UpdateUser(discordId, userForUpdate);
        }

        struct InvitationGroup
        {
            public ulong PlayerOneId { get; set; }
            public ulong PlayerTwoId { get; set; }
        }
    }
}