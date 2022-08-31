#region Packages

using Runtime.Player;
using UnityEngine;

#endregion

namespace Runtime.UI.Overworld
{
    public class InteractIcon : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;

        private PlayerInteractions _playerInteractions;
        private RectTransform rectTransform;
        private Camera cam;

        private void Awake()
        {
            _playerInteractions = PlayerManager.instance.GetInteractions();

            rectTransform = GetComponent<RectTransform>();

            cam = Camera.main;
        }

        private void Update()
        {
            Vector3 worldPos = _playerInteractions.GetFocusedPosition();

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