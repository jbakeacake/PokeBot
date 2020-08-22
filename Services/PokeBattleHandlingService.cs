using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PokeBot.Controllers;
using PokeBot.Dtos;
using PokeBot.PokeBattle;

namespace PokeBot.Services
{
    public class PokeBattleHandlingService
    {
        private readonly DiscordSocketClient _discord;
        public Dictionary<Guid, PokeBattleGame> _currentGames;
        public Dictionary<ulong, (ulong, ulong)> _pendingInvites { get; set; }
        public HashSet<ulong> _busyUsers { get; set; }
        private IServiceProvider _provider;
        private PokemonController _pokeController;
        private UserController _userController;
        public PokeBattleHandlingService(DiscordSocketClient discord, IServiceProvider provider, PokemonController pokemonController, UserController userController)
        {
            _discord = discord;
            _provider = provider;
            _pokeController = pokemonController;
            _userController = userController;
            _busyUsers = new HashSet<ulong>();
            _currentGames = new Dictionary<Guid, PokeBattleGame>();
            _pendingInvites = new Dictionary<ulong, (ulong, ulong)>(); // item1: sender, item2: receiver
            _discord.ReactionAdded += ListenForDuelReactions;
        }

        public void Initialize(IServiceProvider provider)
        {
            _provider = provider;
        }

        private async Task ListenForDuelReactions(Cacheable<IUserMessage, ulong> before, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var message = await before.GetOrDownloadAsync();
            //If the reaction was NOT to a bot message
            if (message.Source != MessageSource.Bot) return;
            //If the reaction was BY this bot
            if (reaction.User.Value.Id == _discord.CurrentUser.Id) return;

            await DoInviteAction(reaction, message);
        }

        private async Task DoInviteAction(SocketReaction reaction, IUserMessage message)
        {
            if (reaction.Emote.Equals(new Emoji("✅")))
            {
                //Get Users:
                var ids = _pendingInvites[message.Id];
                var senderId = ids.Item1;
                var receiverId = ids.Item2;

                //Clean up messages
                _pendingInvites.Remove(message.Id);
                await message.DeleteAsync();
                // Prepare pokemon choices...
                var senderEmbedMessage = await CreateChoosePokemonMessage(senderId);
                var receiverEmbedMessage = await CreateChoosePokemonMessage(receiverId);
                await _discord.GetUser(senderId).SendMessageAsync(embed: senderEmbedMessage);
                await _discord.GetUser(receiverId).SendMessageAsync(embed: receiverEmbedMessage);
                // Create Game Entry for Dictionary:
                Guid battleId = Guid.NewGuid();
                UserForUpdateDto playerOneForUpdate = new UserForUpdateDto(battleId);
                UserForUpdateDto playerTwoForUpdate = new UserForUpdateDto(battleId);

                await _userController.UpdateUser(senderId, playerOneForUpdate);
                await _userController.UpdateUser(receiverId, playerTwoForUpdate);

                PokeBattleGame game = new PokeBattleGame(_pokeController, _userController);
                game.PlayerOne = new BattlePlayer(senderId);
                game.PlayerTwo = new BattlePlayer(receiverId);
                _currentGames.Add(battleId, game);
            }
            else if (reaction.Emote.Equals(new Emoji("❌")))
            {
                _pendingInvites.Remove(message.Id);
                await message.DeleteAsync();
            }
        }

        public async Task InitializePlayerAsync(Guid battleTokenId, ulong discordId, int pokemonId)
        {
            var game = _currentGames[battleTokenId];
            var pokemon = await _pokeController.GetPokemonFromUserInventory(pokemonId);
            var pokeType = await _pokeController.GetPokeType(pokemon.Type);
            
            var playerToUpdate = GetPlayerFromGame(game, discordId);
            playerToUpdate.InitializeCurrentPokemon(pokemon, pokeType, pokemonId);
        }

        private BattlePlayer GetPlayerFromGame(PokeBattleGame game, ulong discordId)
        {
            return discordId == game.PlayerOne.DiscordId ? game.PlayerOne : game.PlayerTwo;
        }

        private async Task<Embed> CreateChoosePokemonMessage(ulong userId)
        {
            var userForReturn = await _userController.GetUserByDiscordId(userId);
            string message = "Type `!choose id#` where 'id#' is the pokemon's corresponding Id.\n\n```╦\n║ Id • Name • Lvl\n╫────────────────────────\n";
            foreach (var pokemon in userForReturn.PokeCollection)
            {
                message += ($"║ {pokemon.Id} • {pokemon.Name} • {pokemon.Level}\n╫────────────────────────\n");
            }
            message += "╩```";

            var embeddedMessage = new EmbedBuilder()
                .WithAuthor(_discord.CurrentUser)
                .WithTitle("▼ Choose your pokemon!")
                .WithDescription(message)
                .WithFooter(footer => footer.Text = "Sent ")
                .WithCurrentTimestamp()
                .Build();

            return embeddedMessage;
        }
    }
}