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

        private PlayerInteractions playerInteractions;
        private RectTransform rectTransform;
        private Camera cam;

        private void Awake()
        {
            this.playerInteractions = this.playerManager.GetInteractions();

            this.rectTransform = this.GetComponent<RectTransform>();

            this.cam = Camera.main;
        }

        private void Update()
        {
            Vector3 worldPos = this.playerInteractions.GetFocusedPosition();

            if (worldPos == Vector3.zero)
            {
                this.visuals.SetActive(false);
                return;
            }

            this.visuals.SetActive(true);

            this.rectTransform.position =  this.cam.WorldToScreenPoint(worldPos);
        }
    }
}