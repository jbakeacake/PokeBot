using System;
using PokeBot.PokeBattle.Entities;

namespace PokeBot.PokeBattle.Moves.Ailments
{
    public abstract class Ailment : IAilment
    {
        public string Name { get; set; }
        public float AilmentChance { get; set; }
        public int ChanceToRecover { get; set; }
        public Ailment(string name, float ailmentChance, int chanceToRecover)
        {
            Name = name;
            AilmentChance = ailmentChance;
            ChanceToRecover = chanceToRecover;
        }

        public bool IsRecoverySuccessful()
        {
            Random rand = new Random();
            var rollToRecover = rand.Next(101);

            return rollToRecover <= ChanceToRecover;
        }
        public abstract void ApplyAilment(ICombative receiver);
        public abstract void RemoveFrom(PokeEntity illPokemon);
    }
}