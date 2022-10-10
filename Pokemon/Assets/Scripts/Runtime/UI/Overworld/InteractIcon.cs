#region Packages

using Runtime.Player;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.UI.Overworld
{
    public class InteractIcon : MonoBehaviour
    {
        [SerializeField, Required] private PlayerManager playerManager;
        
        [SerializeField] private GameObject visuals;

        private PlayerInteractions _playerInteractions;
        private RectTransform rectTransform;
        private Camera cam;

        private void Awake()
        {
            this._playerInteractions = this.playerManager.GetInteractions();

            this.rectTransform = this.GetComponent<RectTransform>();

            this.cam = Camera.main;
        }

        private void Update()
        {
            Vector3 worldPos = this._playerInteractions.GetFocusedPosition();

            if (worldPos == Vector3.zero)
            {
                this.visuals.SetActive(false);
                return;
            }

            this.visuals.SetActive(true);

            Vector3 screenPos = this.cam.WorldToScreenPoint(worldPos);
            this.rectTransform.position = screenPos;
        }
    }
}