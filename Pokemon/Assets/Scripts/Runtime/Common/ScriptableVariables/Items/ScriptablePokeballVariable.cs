#region Packages

using System;
using Runtime.Items.Pokeballs;
using UnityEngine;

#endregion

namespace Runtime.Common.ScriptableVariables.Items
{
    [Serializable]
    [CreateAssetMenu(menuName = "Scriptable Variable/Pokemon Item/Pokeball")]
    public sealed class ScriptablePokeballVariable : ScriptableStruct<int>
    {
        [SerializeField] private Pokeball defaultItem;

        public Pokeball pokeball { get; private set; }
    }
}