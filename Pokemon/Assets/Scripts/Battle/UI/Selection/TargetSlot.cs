#region SDK

using Mfknudsen.Battle.Systems.Spots;
using TMPro;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Battle.UI.Selection
{
    public class TargetSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gui;
        
        private TargetSelection targetSelection;
        private Spot spot;

        #region Getters

        public Spot GetSpot()
        {
            return spot;
        }

        #endregion
        
        public void SetPokemon(TargetSelection targetSelection, Spot spot)
        {
            gameObject.SetActive(true);

            this.targetSelection = targetSelection;
            this.spot = spot;

            // ReSharper disable once Unity.NoNullPropagation
            if (spot?.GetActivePokemon() is null) return;

            gui.text = spot.GetActivePokemon().GetName();
        }

        public void Trigger()
        {
            targetSelection.ReceiveSpot(spot);
        }
    }
}