using System.Collections;
using Runtime.Pokémon;
using UnityEngine;

namespace Runtime.Items
{
    [CreateAssetMenu(fileName = "HoldableItem", menuName = "Item/Create new holdable item")]
    public class HoldableItem : Item
    {
        public override bool IsUsableTarget(Pokemon pokemon)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerator Operation()
        {
            throw new System.NotImplementedException();
        }
    }
}
