#region Packages

using Mfknudsen.Communication;
using Mfknudsen.World.Overworld.Interactions;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen.AI
{
    public abstract class NpcBase : MonoBehaviour, IInteractable
    {
        #region Values

        [FoldoutGroup("Base")] [SerializeField]
        protected Chat idleChat;

        [FoldoutGroup("Base/Visual")] protected GameObject visualsObject;

        #endregion

        #region In

        public abstract void Trigger();

        public void HideVisual()
        {
            visualsObject.SetActive(false);
        }

        public void ShowVisual()
        {
            visualsObject.SetActive(true);
        }

        #endregion
    }
}