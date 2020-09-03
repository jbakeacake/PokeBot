using System.Collections.Generic;
using Discord;

namespace PokeBot.Utils
{
    internal static class MoveSetEmojiMapUtil
    {
        internal static Dictionary<IEmote, int> GetMoveSetEmojiMap
        {
            get
            {
                return new Dictionary<IEmote, int>
                {
                    {new Emoji("1️⃣"), 1},
                    {new Emoji("2️⃣"), 2},
                    {new Emoji("3️⃣"), 3},
                    {new Emoji("4️⃣"), 4},
                };
            }
        }
    }
}