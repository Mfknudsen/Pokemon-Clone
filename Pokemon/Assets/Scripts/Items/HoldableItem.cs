using System.Collections;
using Mfknudsen.Pokémon;
using UnityEngine;

namespace Mfknudsen.Items
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
