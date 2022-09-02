#region Packages

using Runtime.Common;
using UnityEngine;

#endregion

namespace Runtime.UI.Book.Light
{
    public class UIBookLight : MonoBehaviour
    {
        #region Values

        [SerializeField] private int pixelSkip = 10;
        [SerializeField] private float levelToTurnOn;
        [SerializeField] private UnityEngine.Light bookLight;
        [SerializeField] private RenderTexture renderTexture;

        #endregion

        #region In

        public void TurnOff()
        {
            bookLight.enabled = false;
        }

        public void Calculate()
        {
            bookLight.enabled = true;
            
            Texture2D tex = renderTexture.RenderTextureToTexture2D();
            Color[] pixels = tex.GetPixels();

            float brightColor = 0;
            int count = 0;

            for (int i = 0; i < pixels.Length; i += pixelSkip)
            {
                count++;
                brightColor += pixels[i].grayscale;
            }

            bookLight.enabled = brightColor / count * 100f <= levelToTurnOn;
        }

        #endregion
    }
}