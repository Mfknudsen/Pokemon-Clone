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

        #endregion

        #region In

        public abstract void Trigger();

        #endregion
    }
}