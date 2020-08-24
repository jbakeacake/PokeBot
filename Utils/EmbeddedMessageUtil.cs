using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using PokeBot.Dtos;
using PokeBot.PokeBattle;
using PokeBot.PokeBattle.Entities;

namespace PokeBot.Utils
{
    public static class EmbeddedMessageUtil
    {
        public static Embed CreateChoosePokemonMessage(SocketSelfUser bot, UserForReturnDto user)
        {
            string message = "Type `!choose #` where '#' is the pokemon's corresponding Id number.\n\n```╦\n║ Id • Name • Lvl\n╫────────────────────────\n";
            foreach (var pokemon in user.PokeCollection)
            {
                message += ($"║ {pokemon.Id} • {pokemon.Name} • {pokemon.Level}\n╫────────────────────────\n");
            }
            message += "╩```";

            var embeddedMessage = new EmbedBuilder()
                .WithAuthor(bot)
                .WithTitle("▼ Choose your pokemon!")
                .WithDescription(message)
                .WithFooter(footer => footer.Text = "Sent ")
                .WithCurrentTimestamp()
                .Build();

            return embeddedMessage;
        }
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
                .WithFields(new[]{playerOneField, dividerField, playerTwoField})
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
                    $"► {pokemon.Stats.MaxHP} / {pokemon.Stats.MaxHP} HP\n" +
                    $"► {pokemon.Stats.Experience} / {Math.Pow(pokemon.Stats.Level + 1, 3)}"
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
    }
}