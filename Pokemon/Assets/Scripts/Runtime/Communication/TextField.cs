using TMPro;
using UnityEngine;

namespace Runtime.Communication
{
    public class TextField : MonoBehaviour
    {
        public static TextMeshProUGUI instance;

        private void Start()
        {
            if (instance == null)
                instance = this.GetComponent<TextMeshProUGUI>();
        }
    }
}
