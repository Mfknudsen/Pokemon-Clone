#region SDK

using Runtime.Items;
using UnityEngine;

//Custom

#endregion

namespace Runtime.Pokémon
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