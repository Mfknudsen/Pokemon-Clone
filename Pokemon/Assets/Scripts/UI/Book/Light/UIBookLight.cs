#region Packages

using System.Collections;
using Mfknudsen.Common;
using UnityEngine;

#endregion

namespace Mfknudsen.UI.Book.Light
{
    public class UIBookLight : MonoBehaviour
    {
        #region Values

        [SerializeField] private UnityEngine.Light bookLight;
        [SerializeField] private float levelToTurnOn;
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
            
            Texture2D tex = CommonTexture.RenderToTexture2D(renderTexture);
            Color[] pixels = tex.GetPixels();

            float brightColor = 0;
            int count = 0;

            foreach (Color pixel in pixels)
            {
                count++;
                brightColor += (pixel.r + pixel.g + pixel.b) / 3f;
            }

            bookLight.enabled = brightColor / count * 100f <= levelToTurnOn;
        }

        #endregion
    }
}