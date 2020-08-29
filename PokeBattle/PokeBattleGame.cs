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
        public Guid BattleTokenId { get; set; }
        public BattlePlayer PlayerOne { get; set; }
        public BattlePlayer PlayerTwo { get; set; }
        public PokeBattleStateManager StateManager { get; set; }
        public ulong PlayerOneLogId { get; set; }
        public ulong PlayerTwoLogId { get; set; }
        public ulong PlayerOneSceneId { get; set; }
        public ulong PlayerTwoSceneId { get; set; }
        public PokeBattleGame(Guid battleTokenId, PokemonController pokemonController, UserController userController)
        {
            BattleTokenId = battleTokenId;
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
            StateManager = new PokeBattleStateManager(PlayerOne, PlayerTwo);
            await PokemonController.RegisterPokeBattle(pokeBattleForCreationDto);
        }

        public BattlePlayer GetPlayer(ulong discordId)
        {
            if (PlayerOne.DiscordId == discordId)
            {
                return PlayerOne;
            }
            else if (PlayerTwo.DiscordId == discordId)
            {
                return PlayerTwo;
            }
            else
            {
                throw new Exception($"Failed to get Player. Unknown player of id {discordId}.");
            }
        }

        public bool isPlayersTurn(ulong discordId)
        {
            if (PlayerOne.DiscordId == discordId
                && StateManager._currentState == PokeBattleStates.PLAYER_ONE)
            {
                return true;
            }
            else if (PlayerTwo.DiscordId == discordId
                && StateManager._currentState == PokeBattleStates.PLAYER_TWO)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isGameOver()
        {
            return StateManager.isGameOver();
        }
        public bool isGameReady()
        {
            return PlayerOne.CurrentPokemon != null
                && PlayerTwo.CurrentPokemon != null;
        }

        public void RunGame(int moveIndex)
        {
            StateManager.ExecuteTurn(moveIndex);
        }
        public BattlePlayer GetPlayerForMove()
        {
            BattlePlayer playerToReturn = null;
            System.Console.WriteLine("STATE: " + StateManager._currentState);

            if (StateManager._currentState == PokeBattleStates.PLAYER_ONE)
            {
                playerToReturn = StateManager._playerOne;
            }
            else if (StateManager._currentState == PokeBattleStates.PLAYER_TWO)
            {
                playerToReturn = StateManager._playerTwo;
            }

            return playerToReturn;
        }

        public BattlePlayer GetWinningPlayer()
        {
            if (!isGameOver()) return null;

            return StateManager._currentState == PokeBattleStates.PLAYER_ONE_WIN ? PlayerOne : PlayerTwo;
        }

        public BattlePlayer GetLosingPlayer()
        {
            if (!isGameOver()) return null;

            return StateManager._currentState == PokeBattleStates.PLAYER_ONE_WIN ? PlayerTwo : PlayerOne;
        }

        public async Task ClearTokens()
        {
            UserForUpdateDto playerOneForUpdate = new UserForUpdateDto(Guid.Empty);
            UserForUpdateDto playerTwoForUpdate = new UserForUpdateDto(Guid.Empty);

            await UserController.UpdateUser(PlayerOne.DiscordId, playerOneForUpdate);
            await UserController.UpdateUser(PlayerTwo.DiscordId, playerTwoForUpdate);
        }
    }
}