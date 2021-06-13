using TMPro;
using UnityEngine;

namespace Mfknudsen.Battle.UI
{
    public class BattleDisplay : MonoBehaviour
    {
        public TextMeshProUGUI textField = null;

        private void Awake()
        {
            if(textField != null)
            textField.text = "";
        }

        public void DisplayNewText(string newText)
        {
            textField.text = newText;
        }
    }
}