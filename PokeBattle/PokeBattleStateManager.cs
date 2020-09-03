using System;
using PokeBot.PokeBattle.Entities;
using PokeBot.PokeBattle.Moves;

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

        public string ExecuteTurn(int moveIndex)
        {
            switch (_currentState)
            {
                case PokeBattleStates.GAME_START:
                    {
                        doGameStart();
                        break;
                    }
                case PokeBattleStates.PLAYER_ONE:
                    {
                        var logMessage = DoPlayerMove(moveIndex, _playerOne, _playerTwo, nextState: PokeBattleStates.PLAYER_TWO);
                        DetermineConclusion(_playerOne, _playerTwo);
                        return logMessage;
                    }
                case PokeBattleStates.PLAYER_TWO:
                    {
                        var logMessage = DoPlayerMove(moveIndex, _playerTwo, _playerOne, nextState: PokeBattleStates.PLAYER_ONE);
                        DetermineConclusion(_playerOne, _playerTwo);
                        return logMessage;
                    }
                case PokeBattleStates.PLAYER_ONE_WIN:
                    {
                        doPlayerWin(_playerOne, _playerTwo);
                        return "";
                    }
                case PokeBattleStates.PLAYER_TWO_WIN:
                    {
                        doPlayerWin(_playerTwo, _playerOne);
                        return "";
                    }
            }
            return "";
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

        private string DoPlayerMove(int moveIndex, BattlePlayer attacker, BattlePlayer defender, PokeBattleStates nextState)
        {
            var logMessage = "";
            var attackingPokemon = attacker.CurrentPokemon;
            var defendingPokemon = defender.CurrentPokemon;

            var attackerMove = attackingPokemon.Moves[moveIndex - 1];

            var combatActionMessage = "";
            var successfulAttack = attackingPokemon.CombatAction(defendingPokemon, attackerMove, out combatActionMessage);
            var defendingAfflictionMsg = attackingPokemon.TriggerAilments(defendingPokemon);
            var attackingAfflictionMsg = defendingPokemon.TriggerAilments(attackingPokemon);
            if (successfulAttack && !isPlayerDisabled(defender))
            {
                logMessage += $"|{attackingPokemon.Name} used [{attackerMove.Name}]!\n";
                logMessage += combatActionMessage;
                logMessage += CreateEffectLogMessage(attackerMove, attackingPokemon, defendingPokemon);
                logMessage += defendingAfflictionMsg;
                logMessage += attackingAfflictionMsg;
                UpdateState(nextState);
            }


            if (!successfulAttack)
            {
                logMessage += $"|{attackingPokemon.Name} can't use {attackerMove.Name}! It's out of PP.";
            }
            else if (isPlayerDisabled(defender))
            {
                logMessage += $"|{attackingPokemon.Name} used [{attackerMove.Name}]!\n";
                logMessage += combatActionMessage;
                logMessage += CreateEffectLogMessage(attackerMove, attackingPokemon, defendingPokemon);
                logMessage += defendingAfflictionMsg;
                logMessage += attackingAfflictionMsg;
            }

            return logMessage;
        }
        private string CreateEffectLogMessage(Move move, PokeEntity attacker, PokeEntity defender)
        {
            if (move.StatChangeName == "none") return "";
            var logMessage = "";
            if (move.TargetsOther)
            {
                logMessage += $"|{defender.Name}'s {move.StatChangeName} ";
            }
            else
            {
                logMessage += $"|{attacker.Name}'s {move.StatChangeName} ";
            }
            if (move.StatChangeValue > 0)
            {
                logMessage += "rose!\n";
            }
            else
            {
                logMessage += "fell!\n";
            }

            return logMessage;
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