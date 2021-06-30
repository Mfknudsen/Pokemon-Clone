#region SDK

using Mfknudsen.Items;
using UnityEngine;

//Custom

#endregion

namespace Mfknudsen.Pokémon
{
    [CreateAssetMenu(fileName = "MegaStone", menuName = "Item/Create new Mega Stone")]
    public class MegaStone : HoldableItem
    {
        [Header("Mega Stone:")]
        [SerializeField] private Pokemon affectedPokemon = null;

        #region Getters
        public Pokemon GetAffectedPokemon()
        {
            return affectedPokemon;
        }
        #endregion

        #region Setters
        #endregion
    }
}