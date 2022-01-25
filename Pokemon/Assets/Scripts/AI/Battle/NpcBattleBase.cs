#region Packages

using System.Collections;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Communication;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Battle
{
    public class NpcBattleBase : NpcBase
    {
        #region Value

        [FoldoutGroup("Before Battle")] [SerializeField]
        private BattleStarter battleStarter;

        [FoldoutGroup("Before Battle")] [SerializeField]
        private Chat beforeChat;

        #endregion

        #region In

        public override void Trigger()
        {
            if (battleStarter == null) return;

            if (!battleStarter.GetPlayerWon())
                StartCoroutine(BeforeBattle());
            else
                ChatManager.instance.Add(idleChat);
        }

        #endregion

        #region Internal

        private IEnumerator BeforeBattle()
        {
            ChatManager chatManager = ChatManager.instance;
            chatManager.Add(beforeChat);

            yield return null;

            yield return new WaitUntil(() => chatManager.GetIsClear());
        }

        #endregion
    }
}