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

        #region Build In States

        private IEnumerator Start()
        {
            yield return new WaitWhile(() => TimerUpdater.instance == null);

            new Timer(.1f).timerEvent.AddListener(Calculate);
        }
        #endregion

        #region Internal

        private void Calculate()
        {
            if (gameObject.activeInHierarchy)
            {
                Texture2D tex = CommonTexture.RenderToTexture2D(renderTexture);
                Color[] pixels = tex.GetPixels();
                
                float brightColor = 0;
                int count = 0;

                foreach (Color pixel in pixels)
                {
                    count++;
                    brightColor += (pixel.r + pixel.g + pixel.b) / 3f;
                }

                Debug.Log(brightColor / count);

                bookLight.enabled = brightColor / count * 100f <= levelToTurnOn;
            }
            
            new Timer(.5f).timerEvent.AddListener(Calculate);
        }

        #endregion
    }
}