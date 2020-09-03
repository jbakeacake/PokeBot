using System;
using System.Collections.Generic;
using PokeBot.PokeBattle.Moves.Ailments;

namespace PokeBot.Utils
{
    internal static class AilmentAliasMapUtil
    {
        internal static Dictionary<string, Type> GetAilmentAliasMap
        {
            get
            {
                return new Dictionary<string, Type>
                {
                    {"confusion", typeof(Confusion)},
                    {"leech-seed", typeof(Leech)},
                    {"trap", typeof(Disable)},
                    {"sleep", typeof(Disable)},
                    {"paralysis", typeof(Disable)},
                    {"freeze", typeof(Disable)},
                    {"disable", typeof(Disable)},
                    {"poison", typeof(DamageOverTime)},
                    {"burn", typeof(DamageOverTime)}
                };
            }
        }

        internal static Dictionary<string, int> GetDisableChanceToRecoverMap
        {
            get
            {
                return new Dictionary<string, int>
                {
                    {"paralysis", 25},
                    {"sleep", 50},
                    {"freeze", 20},
                    {"disable", 33},
                    {"trap", 33}
                };
            }
        }
    }
}