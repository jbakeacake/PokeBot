using System.Collections.Generic;

namespace PokeBot.PokeBattle.Common
{
    internal static class StageMultiplierUtils
    {
        internal static Dictionary<int, float> GetBasicStatsMap // <StageNumber : int, multiplier : float>
        {
            get
            {
                return new Dictionary<int, float>()
                {
                    {-6, 0.25f},
                    {-5, 0.28f},
                    {-4, 0.33f},
                    {-3, 0.40f},
                    {-2, 0.50f},
                    {-1, 0.66f},
                    {0, 1.0f},
                    {1, 1.5f},
                    {2, 2.0f},
                    {3, 2.5f},
                    {4, 3.0f},
                    {5, 3.5f},
                    {6, 4.0f}
                };
            }
        }
    }
}