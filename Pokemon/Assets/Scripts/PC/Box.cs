#region SDK

using System;
using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using Mfknudsen.UI;
using Mfknudsen.World.Overworld;
using UnityEngine;

#endregion

namespace Mfknudsen.PC
{
    [RequireComponent(typeof(SphereCollider))]
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

        #region Collider

        private void OnTriggerEnter(Collider other)
        {
            PlayerManager.instance.GetInteractions().OnEnter(this, transform);
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerManager.instance.GetInteractions().OnExit(this);
        }

        #endregion
    }
}