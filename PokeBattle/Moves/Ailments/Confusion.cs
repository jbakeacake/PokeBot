using System;
using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle.Moves.Ailments
{
    public class Confusion : Ailment
    {
        private readonly int MAX_TURNS = 4;
        private int TurnCounter { get; set; }
        private readonly float PercentageOfHealth = 0.1f;
        public Confusion(string name, float ailmentChance, int chanceToRecover) : base(name, ailmentChance, chanceToRecover)
        {
        }

        public override void ApplyAilment(PokeEntity receiver)
        {
            if(TurnCounter > 0)
            {
                receiver.SetDisabled(true);
                TurnCounter--;
            }
            else
            {
                RemoveFrom(receiver);
            }
        }

        public override void RemoveFrom(PokeEntity illPokemon)
        {
            if (TurnCounter != 0) return;
            illPokemon.CurrentAilments.Remove(this.Name);
            TurnCounter = MAX_TURNS;
            illPokemon.SetDisabled(false);
        }

        public float GetDamage(PokeEntity illPokemon)
        {
            return illPokemon.Stats.MaxHP * PercentageOfHealth;
        }

        public bool isTargetOther(string pokeType)
        {
            Random rand = new Random();
            // 33% chance to hit self / 66% chance to hit self if fighting type
            int chanceToHitSelf = pokeType == "fighting" ? 66 : 33;
            var roll = rand.Next(1, 101);
            
            return roll >= chanceToHitSelf; // if the roll is greater, we're targeting the other pokemon
        }
    }
}