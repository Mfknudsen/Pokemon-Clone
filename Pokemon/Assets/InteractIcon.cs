#region SDK

using Mfknudsen.Player;
using UnityEngine;

#endregion

namespace Mfknudsen
{
    public class InteractIcon : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;

        private Interactions interactions;
        private RectTransform rectTransform;
        private Camera cam;

        private void Start()
        {
            interactions = PlayerManager.instance.GetInteractions();

            rectTransform = GetComponent<RectTransform>();

            cam = Camera.main;
        }

        private void Update()
        {
            Vector3 worldPos = interactions.GetFocusedPosition();

            if (worldPos == Vector3.zero)
            {
                visuals.SetActive(false);
                return;
            }

            visuals.SetActive(true);

            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
            rectTransform.position = screenPos;
        }
    }
}