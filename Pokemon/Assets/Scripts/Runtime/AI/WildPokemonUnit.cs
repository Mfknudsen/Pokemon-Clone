#region Packages

using Runtime.Pokémon;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.AI
{
    public class WildPokemonUnit : UnitBase
    {
        #region Values

        [SerializeField, Required] private Pokemon pokemon;

        #endregion

        #region Getters

        public Pokemon GetPokemon() => pokemon;

        #endregion

        public override void Trigger()
        {
        }
    }
}