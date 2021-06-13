using TMPro;
using UnityEngine;

namespace Mfknudsen.Chat
{
    public class TextField : MonoBehaviour
    {
        public static TextMeshProUGUI instance = null;

        private void Start()
        {
            if (instance == null)
                instance = GetComponent<TextMeshProUGUI>();
        }

        private void OnDestroy()
        {
            if (instance == GetComponent<TextMeshProUGUI>())
                instance = null;
        }
    }
}
