using System;
using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle
{
    public class PokeBattleStateManager
    {
        public PokeBattleStates _currentState { get; set; }
        public BattlePlayer _playerOne { get; set; }
        public BattlePlayer _playerTwo { get; set; }
        public PokeBattleStateManager(BattlePlayer playerOne, BattlePlayer playerTwo)
        {
            _playerOne = playerOne;
            _playerTwo = playerTwo;
            _currentState = PokeBattleStates.GAME_START;
        }

        private PokeBattleStates DetermineFirstTurn(PokeEntity playerOne, PokeEntity playerTwo)
        {
            return playerOne.Stats.Skills["Speed"].Value >= playerTwo.Stats.Skills["Speed"].Value ? PokeBattleStates.PLAYER_ONE 
                : PokeBattleStates.PLAYER_TWO;
        }

        private void UpdateState(PokeBattleStates state)
        {
            _currentState = state;
        }

        private void DetermineConclusion(BattlePlayer playerOne, BattlePlayer playerTwo)
        {
            var pokemonOne = playerOne.CurrentPokemon;
            var pokemonTwo = playerTwo.CurrentPokemon;
            if (pokemonOne.isDead())
            {
                _currentState = PokeBattleStates.PLAYER_TWO_WIN;
            }
            else if (pokemonTwo.isDead())
            {
                _currentState = PokeBattleStates.PLAYER_ONE_WIN;
            }
        }

        public bool isGameOver()
        {
            return (_currentState == PokeBattleStates.PLAYER_ONE_WIN ||
                _currentState == PokeBattleStates.PLAYER_TWO_WIN);
        }

        public void ExecuteTurn(int moveIndex)
        {
            switch (_currentState)
            {
                case PokeBattleStates.GAME_START:
                    {
                        System.Console.WriteLine("GAME START");
                        doGameStart();
                        break;
                    }
                case PokeBattleStates.PLAYER_ONE:
                    {
                        doPlayerOneMove(moveIndex);
                        UpdateState(PokeBattleStates.PLAYER_TWO);
                        DetermineConclusion(_playerOne, _playerTwo);
                        break;
                    }
                case PokeBattleStates.PLAYER_TWO:
                    {
                        doPlayerTwomove(moveIndex);
                        UpdateState(PokeBattleStates.PLAYER_ONE);
                        DetermineConclusion(_playerOne, _playerTwo);
                        break;
                    }
                case PokeBattleStates.PLAYER_ONE_WIN:
                    {
                        System.Console.WriteLine("PLAYER ONE WIN");
                        doPlayerOneWin();
                        break;
                    }
                case PokeBattleStates.PLAYER_TWO_WIN:
                    {
                        System.Console.WriteLine("PLAYER TWO WIN");
                        doPlayerTwoWin();
                        break;
                    }
            }
        }

        private void doGameStart()
        {
            var next = DetermineFirstTurn(_playerOne.CurrentPokemon, _playerTwo.CurrentPokemon);
            UpdateState(next);
        }

        private void doPlayerOneMove(int moveIndex)
        {
            var pokemon = _playerOne.CurrentPokemon;
            var move = pokemon.Moves[moveIndex-1];
            System.Console.WriteLine($"{pokemon.Name} used {move.Name}");
        }

        private void doPlayerTwomove(int moveIndex)
        {
            var pokemon = _playerTwo.CurrentPokemon;
            var move = pokemon.Moves[moveIndex-1];
            System.Console.WriteLine($"{pokemon.Name} used {move.Name}");
        }

        private void doPlayerTwoWin()
        {
            _playerTwo.RewardWinningPokemonExperience(_playerOne.CurrentPokemon);
        }

        private void doPlayerOneWin()
        {
            _playerOne.RewardWinningPokemonExperience(_playerTwo.CurrentPokemon);
        }
    }
}