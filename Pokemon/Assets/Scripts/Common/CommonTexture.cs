#region Packages

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Mfknudsen.Common
{
    public static class CommonTexture
    {
        public static Texture2D RenderToTexture2D(RenderTexture renderTexture)
        {
            Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            tex.Apply(false);
            Graphics.CopyTexture(renderTexture, tex);
            return tex;
        }
    }
}