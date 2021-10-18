using Mfknudsen.Pokémon;
using UnityEngine;

namespace Mfknudsen.AI.Virtual
{
    public class VirtualPokemon
    {
        private readonly Pokemon pokemon;
        private readonly Pokemon fakePokemon;
        private bool isKnown;


        public VirtualPokemon(Pokemon pokemon)
        {
            this.pokemon = pokemon;
            fakePokemon = Object.Instantiate(pokemon);
            fakePokemon.ResetForAIMemory();
            isKnown = false;
        }

        #region Getters

        public Pokemon GetFakePokemon()
        {
            return fakePokemon;
        }

        public bool GetKnown()
        {
            return isKnown;
        }

        #endregion

        #region In

        public void OnEnterCheck(Pokemon toCheck)
        {
            if (pokemon != toCheck)
                return;

            isKnown = true;
        }

        public void OnAbilityCheck(Pokemon toCheck)
        {
            if (pokemon != toCheck)
                return;
        }

        #endregion
    }
}
