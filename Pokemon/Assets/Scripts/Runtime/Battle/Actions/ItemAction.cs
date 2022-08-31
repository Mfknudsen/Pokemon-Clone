#region Packages

using System.Collections;
using System.Linq;
using Runtime.AI.Battle.Evaluator;
using Runtime.AI.Battle.Evaluator.Virtual;
using Runtime.Battle.Systems;
using Runtime.Battle.Systems.Spots;
using Runtime.Items;
using Runtime.Pokémon;
using Runtime.Systems;
using UnityEngine;

#endregion

namespace Runtime.Battle.Actions
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

        public void SetToUse(Item set)
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

            battleItem.SetTarget(targets[0].GetActivePokemon());
            battleItem.SetOnUse(chatOnActivation);

            OperationsContainer container = new();
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