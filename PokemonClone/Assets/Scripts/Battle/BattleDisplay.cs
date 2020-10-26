using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BattleUI
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