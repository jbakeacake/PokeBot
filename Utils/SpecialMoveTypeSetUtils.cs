using System.Collections.Generic;

namespace PokeBot.Utils
{
    internal static class SpecialMoveTypeSetUtils
    {
        internal static HashSet<string> GetSpecialMoveTypeSet
        {
            get
            {
                return new HashSet<string> {
                    "water",
                    "grass",
                    "fire",
                    "ice",
                    "electric",
                    "psychic",
                    "dragon",
                    "dark"
                };
            }
        }
    }
}