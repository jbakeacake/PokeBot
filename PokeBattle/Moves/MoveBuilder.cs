using System;
using PokeBot.PokeBattle.Moves.Ailments;
using PokeBot.Utils;

namespace PokeBot.PokeBattle.Moves
{
    public class MoveBuilder
    {
        private Move _move { get; set; }
        public MoveBuilder()
        {
            _move = new Move();
        }
        public Move Build()
        {
            return _move;
        }
        public MoveBuilder Name(string name)
        {
            _move.Name = name;
            return this;
        }
        public MoveBuilder StageMultiplierForAccuracy(int multiplier)
        {
            _move.StageMultiplierForAccuracy = multiplier;
            return this;
        }
        public MoveBuilder Accuracy(float accuracy)
        {
            _move.Accuracy = accuracy;
            return this;
        }
        public MoveBuilder EffectChance(float effectChance)
        {
            _move.EffectChance = effectChance;
            return this;
        }
        public MoveBuilder Ailment(string ailmentName)
        {
            //For now let's use placeholder ailments
            _move.Ailment = BuildAilment(ailmentName);
            return this;
        }
        public MoveBuilder Power(float power)
        {
            _move.Power = power;
            return this;
        }
        public MoveBuilder PP(int pp)
        {
            _move.PP = pp;
            return this;
        }
        public MoveBuilder Type(string type)
        {
            _move.Type = type;
            return this;
        }
        public MoveBuilder StatChangeName(string statChangeName)
        {
            _move.StatChangeName = statChangeName;
            return this;
        }
        public MoveBuilder StatChangeValue(int statChangeValue)
        {
            _move.StatChangeValue = statChangeValue;
            return this;
        }
        public MoveBuilder TargetsOther(string targetName)
        {
            _move.TargetsOther = !targetName.ToLower().Equals("user");
            return this;
        }

        private Ailment BuildAilment(string ailmentName)
        {
            if(!AilmentAliasMapUtil.GetAilmentAliasMap.ContainsKey(ailmentName)) return null;
            Type typeOfAilment = AilmentAliasMapUtil.GetAilmentAliasMap[ailmentName];
            var recoveryChance = DetermineChanceToRecover(typeOfAilment, ailmentName);
            Ailment ailmentToReturn = (Ailment)Activator.CreateInstance(typeOfAilment, new object[]{ailmentName, _move.EffectChance, recoveryChance});
            return ailmentToReturn;
        }

        private int DetermineChanceToRecover(Type ailmentType, string ailmentName)
        {
            int chance = 0;
            if(ailmentType.Equals(typeof(Confusion)))
            {
                chance = 33;
            }
            else if (ailmentType.Equals(typeof(Disable)))
            {
                chance = AilmentAliasMapUtil.GetDisableChanceToRecoverMap[ailmentName];
            }

            return chance;
        }
    }
}