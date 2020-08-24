using System;
using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle.Moves.Ailments
{
    public class Disable : Ailment
    {
        public int ChanceToDisable { get; set; } // a number between 0 and 100, determines if the pokemon has a chance to break free of the disable and attack
        public Disable(string name,
            string description,
            float ailmentChance,
            int chanceToDisable,
            int chanceToRecover) : base(name, description, ailmentChance, chanceToRecover)
        {
            ChanceToDisable = chanceToDisable;
        }

        public override void ApplyAilment(ICombative receiver)
        {
            if(isDisabled())
            {
                receiver.SetDisabled(true);   
            }
        }

        public bool isDisabled()
        {
            Random rand = new Random();
            var rollToParalyze = rand.Next(101); // this roll determine if the pokemon is disabled or not (i.e. if they skip their turn)

            return rollToParalyze <= ChanceToDisable;
        }
    }
}