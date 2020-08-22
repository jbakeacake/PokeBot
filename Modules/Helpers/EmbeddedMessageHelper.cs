using Discord;
using Discord.WebSocket;

namespace PokeBot.Modules.Helpers
{
    public static class EmbeddedMessageHelper
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
    }
}