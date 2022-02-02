using UnityEngine;

namespace Mfknudsen.Menu
{
    public class SettingsMaster : MonoBehaviour
    {
        public static SettingsMaster instance;
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
