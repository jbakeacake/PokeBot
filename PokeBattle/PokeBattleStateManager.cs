using System;
using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle
{
    public class PokeBattleStateManager
    {
        public IServiceProvider _provider { get; set; }
        public PokeBattleStates _currentState { get; set; }
        public PokeEntity _playerOne { get; set; }
        public PokeEntity _playerTwo { get; set; }
        public PokeBattleStateManager(PokeEntity playerOne, PokeEntity playerTwo)
        {
            _currentState = DetermineFirstTurn(playerOne, playerTwo);
            _playerOne = playerOne;
            _playerTwo = playerTwo;
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
            if(playerOne.isDead())
            {
                _currentState = PokeBattleStates.PLAYER_TWO_WIN;
            }
            else if (playerTwo.isDead())
            {
                _currentState = PokeBattleStates.PLAYER_ONE_WIN;
            }
        }

        public void Run()
        {
            
        }
    }
}