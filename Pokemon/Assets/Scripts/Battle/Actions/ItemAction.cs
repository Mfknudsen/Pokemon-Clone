#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.AI;
using Mfknudsen.AI.Virtual;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Items;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Actions
{
    [CreateAssetMenu(fileName = "ItemAction", menuName = "Action/Create new Item Action")]
    public class ItemAction : BattleAction
    {
        #region Values

        [Header("Item Action:")] [SerializeField]
        private Item toUse;

        [SerializeField] private BattleMember battleMember;

        #endregion

        #region Getters

        public bool GetToUse()
        {
            return toUse;
        }

        #endregion

        #region Setters

        public void SetToUse(Items.Item set)
        {
            toUse = set;
        }

        public void SetBattleMember(BattleMember set)
        {
            battleMember = set;
        }

        #endregion

        #region Overrides

        public override float Evaluate(Pokemon user, Pokemon target, VirtualBattle virtualBattle,
            PersonalitySetting personalitySetting)
        {
            Debug.LogError("Evaluate Items");

            return 0;
        }


        public override IEnumerator Operation()
        {
            done = false;

            BattleItem battleItem = (BattleItem) toUse;

            foreach (Spot spot in BattleManager.instance.GetSpotOversight().GetSpots()
                         .Where(spot => spot.GetActivePokemon() == currentPokemon))
            {
                battleItem.SetUserName(spot.GetBattleMember().GetName());
                break;
            }

            battleItem.SetTarget(targetPokemon[0]);
            battleItem.SetOnUse(chatOnActivation);

            OperationsContainer container = new OperationsContainer();
            container.Add(toUse);
            OperationManager.instance.AddAsyncOperationsContainer(container);
            while (!toUse.Done())
                yield return null;

            battleMember.GetInventory().RemoveItem(toUse);

            done = true;
        }

        #endregion
    }
}