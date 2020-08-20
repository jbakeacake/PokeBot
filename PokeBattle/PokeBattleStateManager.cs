using System;
using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle
{
    public class PokeBattleStateManager
    {
        public IServiceProvider _provider { get; set; }
        public PokeBattleStates _currentState { get; set; }
        public BattlePlayer _playerOne { get; set; }
        public BattlePlayer _playerTwo { get; set; }
        public PokeBattleStateManager()
        {
            _currentState = PokeBattleStates.GAME_PENDING;
        }

        public void InitializePlayers()
        {
            
        }

        private PokeBattleStates DetermineFirstTurn(PokeEntity playerOne, PokeEntity playerTwo)
        {
            return playerOne.Stats.Skills["Speed"].Value >= playerTwo.Stats.Skills["Speed"].Value ? PokeBattleStates.PLAYER_ONE : PokeBattleStates.PLAYER_TWO;
        }

        private void UpdateState(PokeBattleStates state)
        {
            _currentState = state;
        }

        private void DetermineConclusion(PokeEntity playerOne, PokeEntity playerTwo)
        {
            if (playerOne.isDead())
            {
                _currentState = PokeBattleStates.PLAYER_TWO_WIN;
            }
            else if (playerTwo.isDead())
            {
                _currentState = PokeBattleStates.PLAYER_ONE_WIN;
            }
        }

        public bool isGameOver()
        {
            return (_currentState == PokeBattleStates.PLAYER_ONE_WIN ||
                _currentState == PokeBattleStates.PLAYER_TWO_WIN);
        }

        public void Run()
        {
            switch (_currentState)
            {
                case PokeBattleStates.GAME_PENDING:
                    {
                        break;
                    }
                case PokeBattleStates.GAME_START:
                    {
                        doGameStart();
                        break;
                    }
                case PokeBattleStates.PLAYER_ONE:
                    {
                        break;
                    }
                case PokeBattleStates.PLAYER_TWO:
                    {
                        break;
                    }
                case PokeBattleStates.PLAYER_ONE_WIN:
                    {
                        doPlayerOneWin();
                        break;
                    }
                case PokeBattleStates.PLAYER_TWO_WIN:
                    {
                        doPlayerTwoWin();
                        break;
                    }
            }
        }

        private void doGameStart()
        {
            // var next = DetermineFirstTurn(_playerOne, _playerTwo);
            // UpdateState(next);
        }

        private void doPlayerTwoWin()
        {
            throw new NotImplementedException();
        }

        private void doPlayerOneWin()
        {
            throw new NotImplementedException();
        }
    }
}