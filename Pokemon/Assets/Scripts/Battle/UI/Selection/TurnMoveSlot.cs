#region SDK

using Mfknudsen.Battle.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Battle.UI.Selection
{
    public class TurnMoveSlot : MonoBehaviour
    {
        [SerializeField] private TurnSelection turnSelection;
        [SerializeField] private TextMeshProUGUI gui;
        [SerializeField] private Image background;

        private PokemonMove move;

        public void SetMove(PokemonMove move)
        {
            this.move = move;

            background.color = Color.white;
            gui.text = "";

            if (move is null) return;

            gui.text = move.GetName();

            background.color = move.GetMoveType().GetTypeColor();
        }

        public void Trigger()
        {
            turnSelection.ReceiveAction(move);
        }
    }
}