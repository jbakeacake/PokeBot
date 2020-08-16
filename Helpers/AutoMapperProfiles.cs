using AutoMapper;
using PokeBot.Dtos;
using PokeBot.Models;

namespace PokeBot.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForReturnDto>();
            CreateMap<UserForReturnDto, User>();

            CreateMap<UserForCreationDto, User>();
            CreateMap<User, UserForCreationDto>();

            CreateMap<Pokemon, PokemonForReturnDto>();
            CreateMap<PokemonForReturnDto, Pokemon>();

            CreateMap<PokemonForCreationDto, Pokemon>();
        }
    }
}