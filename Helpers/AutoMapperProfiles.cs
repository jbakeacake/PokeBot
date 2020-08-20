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

            CreateMap<PokemonData, PokemonDataForReturnDto>();
            CreateMap<MoveData, MoveDataForReturnDto>();
            CreateMap<MoveLink, MoveLinkForReturnDto>();

            CreateMap<PokemonDataForCreationDto, PokemonData>();
            CreateMap<MoveDataForCreationDto, MoveData>();
            CreateMap<MoveLinkForCreationDto, MoveLink>();

            CreateMap<PokeType, PokeTypeForReturn>()
                .ForMember(
                    dest => dest.Double_Damage_From,
                    opt => opt.MapFrom(src => src.Delimited_Double_Damage_From.SplitString(','))
                )
                .ForMember(
                    dest => dest.Double_Damage_To,
                    opt => opt.MapFrom(src => src.Delimited_Double_Damage_To.SplitString(','))
                )
                .ForMember(
                    dest => dest.Half_Damage_From,
                    opt => opt.MapFrom(src => src.Delimited_Half_Damage_From.SplitString(','))
                )
                .ForMember(
                    dest => dest.Half_Damage_To,
                    opt => opt.MapFrom(src => src.Delimited_Half_Damage_To.SplitString(','))
                );

        }
    }
}