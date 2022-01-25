using UnityEngine;

namespace Mfknudsen.Menu
{
    public class SettingsMaster : MonoBehaviour
    {
        public static SettingsMaster instance = null;
        [SerializeField] private SettingsData data = null;

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
