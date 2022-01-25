#region Packages

using Mfknudsen.Player;
using Mfknudsen.World.Overworld.Interactions;
using UnityEngine;

#endregion

namespace Mfknudsen.PC
{
    [RequireComponent(typeof(SphereCollider))]
    [AddComponentMenu("Overworld/Interactions")]
    public class Box : MonoBehaviour, IInteractable
    {
        #region In

        public void ShowNextBox()
        {
        }

        public void ShowPreviousBox()
        {
        }

        #endregion

        public void Trigger()
        {
            Debug.Log("Trigger Box");
            //UIManager.instance.SwitchUI(UISelection.Box);
        }
    }
}