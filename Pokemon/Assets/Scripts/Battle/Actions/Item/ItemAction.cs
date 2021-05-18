#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom
using Communications;
#endregion

namespace Battle.Actions.Items
{
    [CreateAssetMenu(fileName = "ItemAction", menuName = "Action/Create new Item Action")]
    public class ItemAction : BattleAction
    {
        #region Values
        [Header("Item Action:")]
        [SerializeField] private Item toUse = null;
        [SerializeField] private Trainer.BattleMember battleMember = null;
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

        public void SetBattleMember(Trainer.BattleMember set)
        {
            battleMember = set;
        }
        #endregion

        #region Overrides
        public override IEnumerator Activate()
        {
            return Operation();
        }

        protected override IEnumerator Operation()
        {
            done = false;
            toUse.SetTarget(currentPokemon);
            //Send Chat
            List<Chat> toSend = new List<Chat>();
            foreach (Chat chat in chatOnActivation)
            {
                Chat c = Instantiate(chat);
                c.AddToOverride("<USER_NAME>", battleMember.GetName());
                c.AddToOverride("<ITEM_NAME>", toUse.GetItemName());
                toSend.Add(c);
            }
            ChatMaster.instance.Add(toSend.ToArray());

            //Activate Item
            BattleMaster.instance.StartCoroutine(toUse.Activate());

            while (!ChatMaster.instance.GetIsClear() || !toUse.GetDone())
                yield return null;

            battleMember.GetInventory().RemoveItem(toUse);

            done = true;
        }
        #endregion
    }
}