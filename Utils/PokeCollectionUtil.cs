using System.Collections.Generic;
using System.Linq;
using PokeBot.Dtos;

namespace PokeBot.Utils
{
    public class PokeCollectionUtil
    {
        public static List<List<PokemonForReturnDto>> SlicePokeCollection(List<PokemonForReturnDto> collection, int maxPerChunk)
        {
            List<List<PokemonForReturnDto>> listOfCollections = new List<List<PokemonForReturnDto>>();
            int numSlices = (collection.Count() / maxPerChunk) - 1; // Subtract 1 to get the number of SLICES, if we didn't we'd be getting the number of PIECES
            int nextListMarker = maxPerChunk; // To boot, items 0-19 will be compiled to a list before starting a new list
            List<PokemonForReturnDto> tmpList = new List<PokemonForReturnDto>();

            collection = collection.OrderBy(x => x.Name).ToList();
            for (int i = 0; i < collection.Count(); i++)
            {
                if (i == nextListMarker)
                {
                    listOfCollections.Add(tmpList);
                    tmpList = new List<PokemonForReturnDto>();
                    nextListMarker += maxPerChunk;
                }
                tmpList.Add(collection[i]);
            }

            //Add the remaining list:
            if(tmpList.Count() != 0) listOfCollections.Add(tmpList);

            return listOfCollections;
        }
        public static string ChunkToString(List<PokemonForReturnDto> collection)
        {
            var message = "```";
            foreach (var pokemon in collection.OrderBy(x => x.Name))
            {
                message += $"◽ {pokemon.Id} | {pokemon.Name.ToUpper()} | Lv. {pokemon.Level}\n";
            }

            message += "```";
            return message;
        }
    }
}