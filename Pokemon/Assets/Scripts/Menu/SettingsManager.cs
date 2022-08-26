using UnityEngine;

namespace Mfknudsen.Menu
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager instance;
        [SerializeField] private SettingsData data;

        private void Start()
        {
            if (instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }
}
