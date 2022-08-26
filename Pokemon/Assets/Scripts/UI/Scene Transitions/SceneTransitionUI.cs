using UnityEngine;

namespace Mfknudsen.UI.Scene_Transitions
{
    public class SceneTransitionUI : MonoBehaviour
    {
        public static SceneTransitionUI instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        public GameObject InstantiateObject(GameObject obj)
        {
            return Instantiate(obj, transform);
        }
    }
}