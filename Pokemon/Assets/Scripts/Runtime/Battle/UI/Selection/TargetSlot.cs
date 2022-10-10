#region SDK

using Runtime.Battle.Systems.Spots;
using TMPro;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Runtime.Battle.UI.Selection
{
    public class TargetSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gui;
        
        private TargetSelection targetSelection;
        private Spot spot;

        #region Getters

        public Spot GetSpot()
        {
            return this.spot;
        }

        #endregion
        
        public void SetPokemon(TargetSelection targetSelection, Spot spot)
        {
            this.gameObject.SetActive(true);

            this.targetSelection = targetSelection;
            this.spot = spot;

            // ReSharper disable once Unity.NoNullPropagation
            if (spot?.GetActivePokemon() is null) return;

            this.gui.text = spot.GetActivePokemon().GetName();
        }

        public void Trigger()
        {
            this.targetSelection.ReceiveSpot(this.spot);
        }
    }
}