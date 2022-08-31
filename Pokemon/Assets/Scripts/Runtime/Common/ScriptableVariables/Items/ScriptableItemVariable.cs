#region Packages

using System;
using Runtime.Items;
using UnityEngine;

#endregion

namespace Runtime.Common.ScriptableVariables.Items
{
    [Serializable]
    [CreateAssetMenu(menuName = "Scriptable Variable/Pokemon Item")]
    public class ScriptableItemVariable : ScriptableStruct<int>
    {
        [field: SerializeField] public Item pokemonItem { get; private set; }

        public static ScriptableItemVariable ConstructNew(int startCount, Item item)
        {
            ScriptableItemVariable result = CreateInstance<ScriptableItemVariable>();
            result.value = startCount;
            result.pokemonItem = item;

            return result;
        }
    }
}