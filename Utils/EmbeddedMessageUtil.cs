using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using PokeBot.Controllers;
using PokeBot.Dtos;
using PokeBot.PokeBattle;
using PokeBot.PokeBattle.Entities;

namespace PokeBot.Utils
{
    public static class EmbeddedMessageUtil
    {
        public static Embed CreateInvitationMessage(SocketSelfUser self, SocketUser sender, SocketUser receiver)
        {
            var embeddedMessage = new EmbedBuilder()
                .WithAuthor(self)
                .WithTitle($"Duel Invitation for {receiver.Username}")
                .WithDescription($"{sender.Username} has sent you, {receiver.Mention}, an invite to duel!\n\nPress ✅ to accept\nPress ❌ to decline\n\n")
                .WithFooter(footer => footer.Text = "Sent ")
                .WithThumbnailUrl("https://i.etsystatic.com/15539528/r/il/da9a71/1266529816/il_570xN.1266529816_5abl.jpg")
                .WithCurrentTimestamp()
                .Build();
            return embeddedMessage;
        }
        public static Embed CreateAwaitingOpponentEmbed(SocketSelfUser self)
        {
            return new EmbedBuilder()
                .WithAuthor(self)
                .WithTitle("Waiting for other player…")
                .WithImageUrl("https://res.cloudinary.com/jbakeacake/image/upload/v1598238947/test/UnknownPlayer_yvpyjh.png")
                .WithFooter(footer => footer.Text = "Waiting Since ")
                .WithCurrentTimestamp()
                .Build();
        }
        public static Embed CreateVersusPokemonEmbed(SocketSelfUser self, BattlePlayer playerOne, BattlePlayer playerTwo)
        {
            var p1 = playerOne.CurrentPokemon;
            var p2 = playerTwo.CurrentPokemon;
            var playerOneField = CreateFieldForPokemonCard(p1);
            var playerTwoField = CreateFieldForPokemonCard(p2);
            var dividerField = new EmbedFieldBuilder()
                .WithName("▐|▌")
                .WithValue(@"
                ▐|▌
                ▐|▌
                ▐|▌
                ▐|▌")
                .WithIsInline(true);

            return new EmbedBuilder()
                .WithAuthor(self)
                .WithTitle(playerOne.CurrentPokemon.Name.ToUpper() + " VS. " + playerTwo.CurrentPokemon.Name.ToUpper())
                .WithFields(new[] { playerOneField, dividerField, playerTwoField })
                .WithColor(Color.Red)
                .WithDescription("Match is Starting NOW!")
                .Build();
        }

        private static EmbedFieldBuilder CreateFieldForPokemonCard(PokeEntity pokemon)
        {
            var fieldBuilder = new EmbedFieldBuilder()
                .WithName($"{pokemon.Name.ToUpper()}")
                .WithValue(
                    $"► Lv. {pokemon.Stats.Level}\n" +
                    $"► `{pokemon.PokeType.Name}`\n" +
                    $"► {(int)pokemon.Stats.MaxHP} / {(int)pokemon.Stats.MaxHP} HP\n" +
                    $"► {pokemon.Stats.Experience} / {Math.Pow(pokemon.Stats.Level + 1, 3)}\n" +
                    $"1. {pokemon.Moves[0].Name}\n2. {pokemon.Moves[1].Name}\n3. {pokemon.Moves[2].Name}\n4. {pokemon.Moves[3].Name}"
                    )
                .WithIsInline(true);
            return fieldBuilder;
        }
        public static Embed CreateWaitingForOtherPlayerEmbed(SocketSelfUser self, PokemonForReturnDto chosenPokemon, string thumbnailUrl)
        {
            return new EmbedBuilder()
                .WithAuthor(self)
                .WithTitle("Your Opponent is waiting on you…")
                .WithThumbnailUrl(thumbnailUrl)
                .WithDescription($"Your Opponent's Pokemon:\n" +
                    $"► {chosenPokemon.Name}\n" +
                    $"► Lv. {chosenPokemon.Level}\n" +
                    $"► `{chosenPokemon.Type}`\n" +
                    $"► {chosenPokemon.MaxHP} / {chosenPokemon.MaxHP} HP\n" +
                    $"► {chosenPokemon.Experience} / {Math.Pow(chosenPokemon.Level + 1, 3)}")
                .WithFooter(footer => footer.Text = "Waiting Since ")
                .WithCurrentTimestamp()
                .Build();
        }

        public static async Task<Embed> CreateBattleSceneEmbed(SocketSelfUser self, BattlePlayer playerInControl, BattlePlayer opponent, PokemonController pokemonController)
        {
            var controllerPokemon = playerInControl.CurrentPokemon;
            var opponentPokemon = opponent.CurrentPokemon;
            var backFacingSprite = (await pokemonController.GetPokemonData(controllerPokemon.PokeId)).BackSpriteUrl;
            var frontFacingSprite = (await pokemonController.GetPokemonData(opponentPokemon.PokeId)).FrontSpriteUrl;

            string controllerHealthbar = GetPokemonHealthBar(controllerPokemon);
            string opponentHealthBar = GetPokemonHealthBar(opponentPokemon);

            var opponentField = new EmbedFieldBuilder()
                .WithName($"`{opponentHealthBar}`\n{(int)opponentPokemon.Stats.HP}/{(int)opponentPokemon.Stats.MaxHP}")
                .WithValue($"`{opponentPokemon.Name} Lv. {opponentPokemon.Stats.Level}`");
            var controllerField = new EmbedFieldBuilder()
                .WithName($"`{controllerHealthbar}`\n {(int)controllerPokemon.Stats.HP}/{(int)controllerPokemon.Stats.MaxHP}")
                .WithValue($"`{controllerPokemon.Name} Lv. {controllerPokemon.Stats.Level}`")
                .WithIsInline(true);
            var moves = "▄▄▄▄▄▄▄▄▄▄▄\n\n►► Moves ◄◄\n";
            int counter = 1;
            foreach(var move in controllerPokemon.Moves)
            {
                moves += $"║ {counter}. {move.Name} :: {move.PP} PP\n";
                counter++;
            }
            var fieldEmpty1 = new EmbedFieldBuilder()
                .WithName(".")
                .WithValue(".");
            var fieldEmpty2 = new EmbedFieldBuilder()
                .WithName(".")
                .WithValue(".");
            var fieldEmpty3 = new EmbedFieldBuilder()
                .WithName(".")
                .WithIsInline(true)
                .WithValue(".");
            var fieldEmpty4 = new EmbedFieldBuilder()
                .WithName(".")
                .WithIsInline(true)
                .WithValue(".");

            return new EmbedBuilder()
                .WithFields(new[] {opponentField, fieldEmpty1, fieldEmpty2, fieldEmpty3, fieldEmpty4, controllerField})
                .WithFooter(footer => footer.Text = moves)
                .WithThumbnailUrl(frontFacingSprite)
                .WithImageUrl(backFacingSprite)
                .Build();
        }

        private static string GetPokemonHealthBar(PokeEntity pokemon)
        {
            var percent = (pokemon.Stats.HP / pokemon.Stats.MaxHP) * 10; // multiply by 10 for nice round numbers
            string healthBar = "|";
            int numberOfHealthBars = (int)percent;
            for (; numberOfHealthBars > 0; numberOfHealthBars--)
            {
                healthBar += "█ ";
            }
            return healthBar;
        }

        public static Embed CreateWinningPlayerEmbed(SocketSelfUser self, BattlePlayer player)
        {
            var pokemon = player.CurrentPokemon;
            var pokemonStats = pokemon.Stats.ToString();
            return new EmbedBuilder()
                .WithAuthor(self)
                .WithTitle("You Won!")
                .WithDescription($"Your pokemon gained ALOT of experience!\n**{pokemon.Name.ToUpper()}**: \n{pokemonStats}")
                .WithImageUrl("https://garyland.neocities.org/images/kingGary.jpg")
                .WithFooter(footer => footer.Text = "Winner at ")
                .WithCurrentTimestamp()
                .Build();
        }

        public static Embed CreateLosingPlayerEmbed(SocketSelfUser self, BattlePlayer player)
        {
            var pokemon = player.CurrentPokemon;
            var pokemonStats = pokemon.Stats.ToString();
            return new EmbedBuilder()
                .WithAuthor(self)
                .WithTitle("You whited out!")
                .WithDescription($"Your pokemon gained SOME experience...\n**{pokemon.Name.ToUpper()}**: \n{pokemonStats}")
                .WithFooter(footer => footer.Text = "Loser at ")
                .WithCurrentTimestamp()
                .Build();
        }

        public static Embed CreatePokemonDetailEmbed(SocketSelfUser self, PokeEntity pokemon)
        {
            var pokemonStats = pokemon.Stats.ToString();
            return new EmbedBuilder()
                .WithAuthor(self)
                .WithColor(Color.Red)
                .WithTitle($"🎴 POKÉDEX")
                .WithThumbnailUrl(pokemon.FrontSpriteUrl)
                .WithDescription($"▼**{pokemon.Name.ToUpper()}**\n{pokemonStats}\n\n ►► Moves ◄◄ \n 1.) {pokemon.Moves[0].Name}\n2.) {pokemon.Moves[1].Name}\n3.) {pokemon.Moves[2].Name}\n4.) {pokemon.Moves[3].Name}")
                .WithFooter(footer => footer.Text = "Detailed at ")
                .WithCurrentTimestamp()
                .Build();
        }
    }
}