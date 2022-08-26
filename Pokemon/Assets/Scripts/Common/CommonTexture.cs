#region Packages

using UnityEngine;

#endregion

namespace Mfknudsen.Common
{
    public static class CommonTexture
    {
        public static Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
        {
            Texture2D tex = new(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            RenderTexture.active = renderTexture;
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();
            return tex;
        }
    }
}