#region Packages

using Mfknudsen.Player;
using Mfknudsen.World.Overworld.Interactions;
using UnityEngine;

#endregion

namespace Mfknudsen.PC
{
    [RequireComponent(typeof(SphereCollider))]
    [AddComponentMenu("Overworld/Interactions")]
    public class Box : InteractItem
    {
        #region In

        public void ShowNextBox()
        {
        }

        public void ShowPreviousBox()
        {
        }

        #endregion

        public override void Trigger()
        {
            Debug.Log("Trigger Box");
            //UIManager.instance.SwitchUI(UISelection.Box);
        }

        #region Collider

        private void OnTriggerEnter(Collider other)
        {
            PlayerManager.Instance.GetInteractions().OnEnter(this, transform);
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerManager.Instance.GetInteractions().OnExit(this);
        }

        #endregion
    }
}