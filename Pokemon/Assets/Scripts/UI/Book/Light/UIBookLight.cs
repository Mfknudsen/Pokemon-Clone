#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Mfknudsen.Common;
using UnityEngine;

#endregion

namespace Mfknudsen.Player.UI_Book
{
    public class UIBookLight : MonoBehaviour
    {
        #region Values

        [SerializeField] private UnityEngine.Camera renderCamera;
        [SerializeField] private Light light;
        [SerializeField] private float levelToTurnOn;

        #endregion

        #region Build In States

        private IEnumerator Start()
        {
            yield return new WaitWhile(() => TimerUpdater.instance == null);

            new Timer(.1f).timerEvent.AddListener(() => Calc());
        }

        #endregion

        #region Internal

        private void Calc()
        {
            RenderTexture renderTexture = new RenderTexture(renderCamera.pixelWidth, renderCamera.pixelHeight, 24);
            renderCamera.targetTexture = renderTexture;
            renderCamera.Render();
            Texture2D tex = CommonTexture.RenderToTexture2D(renderTexture);

            renderCamera.targetTexture = null;

            float brightColor = 0;
            int count = 0;

            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i = i + 5)
            {
                count = count + 1;
                brightColor = brightColor + (pixels[i].r + pixels[i].g + pixels[i].b) / 3;
            }

            Debug.Log(brightColor / count);

            light.enabled = brightColor / count * 100f <= levelToTurnOn;

            new Timer(.2f).timerEvent.AddListener(() => Calc());
        }

        #endregion
    }
}