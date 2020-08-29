using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Webhook;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PokeBot.Controllers;
using PokeBot.Dtos;
using PokeBot.Helpers;
using PokeBot.Models;
using PokeBot.PokeBattle.Entities;
using PokeBot.PokeBattle.Moves;
using PokeBot.Utils;

namespace PokeBot.Modules
{
    public class PokeModule : ModuleBase<SocketCommandContext>
    {
        public UserController _userController { get; set; }
        public PokemonController _pokemonController { get; set; }
        public DiscordSocketClient _discord { get; set; }
        public IServiceProvider _provider { get; set; }
        public PokeModule(IServiceProvider provider, DiscordSocketClient discord, UserController userController, PokemonController pokemonController)
        {
            _discord = discord;
            _userController = userController;
            _pokemonController = pokemonController;
            _provider = provider;
        }
        [Command("catch")]
        public async Task Catch()
        {
            var cwp = _provider.GetRequiredService<CurrentWanderingPokemon>();
            if (cwp._isCaptured)
            {
                await ReplyAsync("There aren't any pokemon around right now. Watch out for the next one!");
                return;
            }

            var user = Context.Message.Author;
            if (!(await _userController.UserExists(user.Id)))
                await RegisterUser(user);

            PokemonForCreationDto pokemonForCreation = CreatePokemon(cwp._pokemon);
            cwp.SetIsCaptured(true);

            await _userController.AddToUserPokeCollectionByDiscordId(user.Id, pokemonForCreation);
            await ReplyAsync($":medal: Congratulations, {user.Username}! You've successfully caught a `{cwp._pokemon.Name}`.\n\nType `!inventory` to see it in your inventory!");
        }

        [Command("inventory")]
        public async Task Inventory()
        {
            var user = Context.Message.Author;
            if (!(await _userController.UserExists(user.Id)))
            {
                await RegisterUser(user);
                await ReplyAsync($"Your inventory is empty. Go catch some pokemon!");
                return;
            }

            var userData = await _userController.GetUserByDiscordId(user.Id);
            var uniqueCount = userData.PokeCollection.Select(x => x.Name).Distinct().Count();
            var message = $":globe_with_meridians:` HELLO, {user.Username.ToUpper()}. WELCOME TO YOUR POKEMON STORAGE UNIT. \nBELOW YOU CAN FIND A LIST OF ALL THE POKEMON YOU'VE CAUGHT...`\n\n";
            message += $":ballot_box:`POKE STORAGE | Unique Pokemon Found: {uniqueCount} / 151`\n";
            message += InventoryToString(userData);

            var res = await ReplyAsync(message);
        }

        [Command("detail")]
        public async Task Detail(int id)
        {
            var user = Context.Message.Author;
            if (!(await _userController.UserExists(user.Id)))
            {
                await RegisterUser(user);
                await ReplyAsync($"Your inventory is empty. Go catch some pokemon!");
                return;
            }

            var userData = await _userController.GetUserByDiscordId(user.Id);

            if(!userData.PokeCollection.Any(x => x.Id == id)) return;

            var pokemonForReturn = userData.PokeCollection.FirstOrDefault(x => x.Id == id);
            var pokeTypeForReturn = await _pokemonController.GetPokeType(pokemonForReturn.Type);
            var moves = await GetMovesFromIds(pokemonForReturn.MoveIds);
            var pokemon = new PokeEntity(id, pokemonForReturn, pokeTypeForReturn, moves);
        
            var embeddedMessage = EmbeddedMessageUtil.CreatePokemonDetailEmbed(Context.Client.CurrentUser, pokemon);
            await user.SendMessageAsync(embed: embeddedMessage);
        }

        private string InventoryToString(UserForReturnDto userData)
        {
            var message = "```";
            foreach (var pokemon in userData.PokeCollection.OrderBy(x => x.Name))
            {
                message += $"◽ {pokemon.Id} | {pokemon.Name.ToUpper()} | Lv. {pokemon.Level}\n";
            }

            message += "```";
            return message;
        }

        private async Task RegisterUser(SocketUser user)
        {
            await _userController.RegisterUser(user.Id, user.Username);
            System.Console.WriteLine($"Registered user {user.Username}");
        }

        private PokemonForCreationDto CreatePokemon(PokemonDataForReturnDto pokemonDataForReturn)
        {
            //Get a random set of moves for our pokemon
            int[] moveIds = GetRandomMoveIds(pokemonDataForReturn.MoveLinks);
            //Create our pokemon:
            PokemonForCreationDto pokemonForCreation = new PokemonForCreationDto
            {
                PokeId = pokemonDataForReturn.PokeId,
                Name = pokemonDataForReturn.Name,
                MaxHP = pokemonDataForReturn.MaxHP,
                Level = pokemonDataForReturn.Level,
                Base_Experience = pokemonDataForReturn.Base_Experience,
                Experience = pokemonDataForReturn.Experience,
                Attack = pokemonDataForReturn.Attack,
                Defense = pokemonDataForReturn.Defense,
                SpecialAttack = pokemonDataForReturn.SpecialAttack,
                SpecialDefense = pokemonDataForReturn.SpecialDefense,
                Speed = pokemonDataForReturn.Speed,
                Type = pokemonDataForReturn.Type,
                MoveId_One = moveIds[0],
                MoveId_Two = moveIds[1],
                MoveId_Three = moveIds[2],
                MoveId_Four = moveIds[3]
            };

            return pokemonForCreation;
        }

        private int[] GetRandomMoveIds(ICollection<MoveLink> moveLinks)
        {
            int[] moveIds = new int[4];
            Random rand = new Random();
            var moveCount = moveLinks.Count();
            int skips = rand.Next(1, moveCount+1);
            for(int i = 0; i < moveIds.Length; i++)
            {
                moveIds[i] = moveLinks.Skip(skips).Take(1).First().MoveId;
                skips = rand.Next(1, moveCount+1);
            }

            return moveIds;
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