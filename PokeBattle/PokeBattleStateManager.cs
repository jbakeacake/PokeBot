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
            return playerOne.Stats.Skills["speed"].Value >= playerTwo.Stats.Skills["speed"].Value ? PokeBattleStates.PLAYER_ONE
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
                        DoPlayerMove(moveIndex, _playerOne, _playerTwo);
                        var nextState = isPlayerDisabled(_playerTwo) ? PokeBattleStates.PLAYER_ONE : PokeBattleStates.PLAYER_TWO;
                        _playerTwo.CurrentPokemon.TryToRecoverAilments();
                        UpdateState(nextState);
                        DetermineConclusion(_playerOne, _playerTwo);
                        break;
                    }
                case PokeBattleStates.PLAYER_TWO:
                    {
                        DoPlayerMove(moveIndex, _playerTwo, _playerOne);
                        var nextState = isPlayerDisabled(_playerOne) ? PokeBattleStates.PLAYER_TWO : PokeBattleStates.PLAYER_ONE;
                        UpdateState(nextState);
                        _playerOne.CurrentPokemon.TryToRecoverAilments();
                        DetermineConclusion(_playerOne, _playerTwo);
                        break;
                    }
                case PokeBattleStates.PLAYER_ONE_WIN:
                    {
                        System.Console.WriteLine("PLAYER ONE WIN");
                        doPlayerWin(_playerOne, _playerTwo);
                        break;
                    }
                case PokeBattleStates.PLAYER_TWO_WIN:
                    {
                        System.Console.WriteLine("PLAYER TWO WIN");
                        doPlayerWin(_playerTwo, _playerOne);
                        break;
                    }
            }
        }

        private bool isPlayerDisabled(BattlePlayer player)
        {
            return player.CurrentPokemon.Disabled;
        }

        private void doGameStart()
        {
            var next = DetermineFirstTurn(_playerOne.CurrentPokemon, _playerTwo.CurrentPokemon);
            UpdateState(next);
        }

        private void DoPlayerMove(int moveIndex, BattlePlayer attacker, BattlePlayer defender)
        {
            var attackingPokemon = attacker.CurrentPokemon;
            var defendingPokemon = defender.CurrentPokemon;

            var attackerMove = attackingPokemon.Moves[moveIndex - 1];

            attackingPokemon.CombatAction(defendingPokemon, attackerMove);
            attackingPokemon.TriggerAilments(defendingPokemon);
            defendingPokemon.TriggerAilments(attackingPokemon);
        }
        private void doPlayerWin(BattlePlayer winner, BattlePlayer loser)
        {
            winner.RewardWinningPokemonExperience(loser.CurrentPokemon);
            loser.RewardLosingPokemonExperience(loser.CurrentPokemon);
            winner.DeterminePokemonLevelUp();
            loser.DeterminePokemonLevelUp();
        }
    }
}