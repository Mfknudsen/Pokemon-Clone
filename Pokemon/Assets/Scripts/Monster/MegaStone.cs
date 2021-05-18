#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom
using Monster;
#endregion

namespace Monster
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