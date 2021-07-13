#region SDK

using Mfknudsen.Battle.Systems;
using Mfknudsen.Pokémon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Mfknudsen.Battle.UI.Selection
{
    public class TargetSlot : MonoBehaviour
    {
        private TargetSelection targetSelection;
        private Spot spot;

        [SerializeField] private TextMeshProUGUI gui; 
        
        public void SetPokemon(TargetSelection targetSelection, Spot spot)
        {
            gameObject.SetActive(true);
            
            this.targetSelection = targetSelection;
            this.spot = spot;

            gui.text = spot.GetActivePokemon().GetName();
        }

        public void Trigger()
        {
            targetSelection.ReceiveSpot(spot);
        }
    }
}
