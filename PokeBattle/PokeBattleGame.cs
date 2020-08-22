using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeBot.Controllers;
using PokeBot.Dtos;

namespace PokeBot.PokeBattle
{
    public class PokeBattleGame
    {
        public PokemonController PokemonController { get; set; }
        public UserController UserController { get; set; }
        public BattlePlayer PlayerOne { get; set; }
        public BattlePlayer PlayerTwo { get; set; }
        public PokeBattleStateManager StateManager { get; set; }
        public PokeBattleGame(PokemonController pokemonController, UserController userController)
        {
            PokemonController = pokemonController;
            UserController = userController;
        }

        public async Task InitializeAsync(Guid battleTokenId)
        {
            if (PlayerOne == null || PlayerTwo == null) return;
            PokeBattleForCreationDto pokeBattleForCreationDto = new PokeBattleForCreationDto
            (
                battleTokenId,
                PlayerOne.DiscordId,
                PlayerTwo.DiscordId
            );

            await PokemonController.RegisterPokeBattle(pokeBattleForCreationDto);
        }

        public bool isGameOver()
        {
            return StateManager.isGameOver();
        }

        public void StartGame()
        {
            System.Console.WriteLine("INITIATE GAME");
        }
    }
}