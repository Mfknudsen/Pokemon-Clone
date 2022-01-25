#region SDK

using Mfknudsen.Pokémon;
using UnityEngine;
using UnityEngine.UI; //Custom

#endregion

namespace Mfknudsen.PC
{
    public class BoxSpot : MonoBehaviour
    {
        private Pokemon onSpot = null;
        private Image visual = null;

        public void SetPokemon(Pokemon set)
        {
            onSpot = set;

            visual.sprite = null;
        }
    }
}