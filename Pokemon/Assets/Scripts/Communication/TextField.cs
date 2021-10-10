using TMPro;
using UnityEngine;

namespace Mfknudsen.Communication
{
    public class TextField : MonoBehaviour
    {
        public static TextMeshProUGUI instance;

        private void Start()
        {
            if (instance == null)
                instance = GetComponent<TextMeshProUGUI>();
        }
    }
}
