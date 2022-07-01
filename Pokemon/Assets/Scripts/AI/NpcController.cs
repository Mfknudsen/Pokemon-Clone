#region Packages

using System.Collections;
using NodeCanvas.BehaviourTrees;
using UnityEngine;

#endregion

namespace Mfknudsen.AI
{
    public class NpcController : MonoBehaviour
    {
        #region Values

        [SerializeField] private BehaviourTreeOwner bto;

        #endregion

        #region Build In States

        private IEnumerator Start()
        {
            yield return new WaitWhile(() => NpcManager.instance == null);
            
            NpcManager.instance.AddController(this);
        }

        private void OnDestroy()
        {
            NpcManager.instance.RemoveController(this);
        }

        #endregion

        #region In

        public void TriggerBehaviourUpdate()
        {
            bto.UpdateBehaviour();
        }

        #endregion
    }
}