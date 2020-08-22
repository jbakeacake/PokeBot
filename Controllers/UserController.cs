using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using PokeBot.Data;
using PokeBot.Dtos;
using PokeBot.Models;

namespace PokeBot.Controllers
{
    public class UserController
    {
        private readonly IPokeRepository _repo;
        private readonly IMapper _mapper;
        public UserController(IPokeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task RegisterUser(ulong discordId, string name)
        {
            UserForCreationDto userForRepo = new UserForCreationDto(discordId, name);
            User user = _mapper.Map<User>(userForRepo);
            _repo.Add(user);
            var res = await _repo.SaveAll();

            if(!res) throw new Exception($"Failed to save on creation of user {discordId}");

            Console.WriteLine($"User {discordId} successfully registered.");
        }

        public async Task<bool> UserExists(ulong discordId)
        {
            return await _repo.UserExists(discordId);
        }

        public async Task<UserForReturnDto> GetUser(int id)
        {
            var userFromRepo = await _repo.GetUser(id);
            var userForReturnDto = _mapper.Map<UserForReturnDto>(userFromRepo);
            return userForReturnDto;
        }

        public async Task<UserForReturnDto> GetUserByDiscordId(ulong discordId)
        {
            var userFromRepo = await _repo.GetUserByDiscordId(discordId);
            var userForReturnDto = _mapper.Map<UserForReturnDto>(userFromRepo);
            return userForReturnDto;
        }

        public async Task<IEnumerable<UserForReturnDto>> GetUsersByBattleTokenId(Guid battleTokenId)
        {
            var usersFromRepo = await _repo.GetUsersByBattleTokenId(battleTokenId);
            var usersForReturn = _mapper.Map<IEnumerable<UserForReturnDto>>(usersFromRepo);
            return usersForReturn;
        }

        public async Task AddToUserPokeCollection(int id, PokemonForCreationDto pokemonForUpdateDto)
        {
            var userFromRepo = await _repo.GetUser(id);

            if (userFromRepo == null) return;

            var pokemon = _mapper.Map<Pokemon>(pokemonForUpdateDto);
            userFromRepo.PokeCollection.Add(pokemon);

            var res = await _repo.SaveAll();
            if (!res) throw new Exception($"Updating user {id}. Failed to save to database!");
        }
        public async Task AddToUserPokeCollectionByDiscordId(ulong discordId, PokemonForCreationDto pokemonForUpdateDto)
        {
            var userFromRepo = await _repo.GetUserByDiscordId(discordId);

            if (userFromRepo == null) return;

            var pokemon = _mapper.Map<Pokemon>(pokemonForUpdateDto);
            userFromRepo.PokeCollection.Add(pokemon);

            var res = await _repo.SaveAll();
            if (!res) throw new Exception($"Updating user {discordId}. Failed to save to database!");
        }

        public async Task UpdateUser(ulong discordId, UserForUpdateDto userForUpdateDto)
        {
            if(!(await _repo.UserExists(discordId))) return;

            var userFromRepo = await _repo.GetUserByDiscordId(discordId);
            _mapper.Map(userForUpdateDto, userFromRepo);

            if(await _repo.SaveAll())
                return;
            
            throw new Exception($"Updating user {discordId} failed on save...");
        }
    }
}