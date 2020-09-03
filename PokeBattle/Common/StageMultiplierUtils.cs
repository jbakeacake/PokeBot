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
                    {1, 1.2f},
                    {2, 1.4f},
                    {3, 1.6f},
                    {4, 1.8f},
                    {5, 2.0f},
                    {6, 2.2f}
                };
            }
        }
    }
}