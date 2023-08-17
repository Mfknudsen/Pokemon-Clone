#region Libraries

using System.Collections;
using TMPro;
using UnityEngine;

#endregion

namespace Runtime.UI.TextEffects
{
    public sealed class TextFade 
    {
        #region Values

        public enum FadeType
        {
            All,
            LeftToRight,
            RightToLeft,
            TopToButton,
            ButtonToTop
        }

        private float highestX, highestY;
        private float lowestX, lowestY;

        private float diffX, diffY;

        private TMP_TextInfo textInfo;

        private FadeType fadeType;

        private Coroutine coroutine;

        private MonoBehaviour owner;

        private float timeToComplete;

        private bool paused, destroyOnDone;

        #endregion

        #region Build In States

        public TextFade(TMP_TextInfo textInfo, float timeToComplete, FadeType fadeType, MonoBehaviour owner,
            bool destroyOnDone = false)
        {
            this.textInfo = textInfo;
            this.fadeType = fadeType;
            this.owner = owner;
            this.timeToComplete = timeToComplete;

            this.coroutine = null;

            this.highestX = 0;
            this.highestY = 0;
            this.lowestX = 0;
            this.lowestY = 0;
            this.diffX = 0;
            this.diffY = 0;

            this.paused = false;
            this.destroyOnDone = destroyOnDone;

            this.SetLowsAndHighs();

            this.coroutine = owner.StartCoroutine(this.Effect());
        }

        #endregion

        #region Getters

        public float LowestX() => this.lowestX;

        public float LowestY() => this.lowestY;

        public float HighestX() => this.highestX;

        public float HighestY() => this.highestY;

        public TMP_TextInfo TextInfo() => this.textInfo;

        #endregion

        #region Setters

        public void SetFadeType(FadeType set)
        {
            this.fadeType = set;
            this.SetLowsAndHighs();
        }

        #endregion

        #region In

        public void Pause() =>
            this.paused = true;

        public void Resume() =>
            this.paused = false;

        #endregion

        #region Internal

        private IEnumerator Effect()
        {
            yield break;
        }

        private void SetLowsAndHighs()
        {
            switch (this.fadeType)
            {
                case FadeType.All:
                    return;

                case FadeType.LeftToRight or FadeType.RightToLeft:
                {
                    this.lowestX = this.textInfo.meshInfo[this.textInfo.characterInfo[0].materialReferenceIndex]
                        .vertices[0].x;
                    this.highestX = this.textInfo.meshInfo[this.textInfo.characterInfo[1].materialReferenceIndex]
                        .vertices[4].x;

                    for (int i = 1; i < this.textInfo.characterCount; i++)
                    {
                        TMP_CharacterInfo charInfo = this.textInfo.characterInfo[i];
                        if (!charInfo.isVisible)
                            continue;
                        float newLow = this.textInfo.meshInfo[charInfo.materialReferenceIndex]
                            .vertices[charInfo.vertexIndex].x;

                        if (this.lowestX > newLow) this.lowestX = newLow;
                    }

                    for (int i = 1; i < this.textInfo.characterCount; i++)
                    {
                        TMP_CharacterInfo charInfo = this.textInfo.characterInfo[i];
                        if (!charInfo.isVisible)
                            continue;
                        float newHigh = this.textInfo.meshInfo[charInfo.materialReferenceIndex]
                            .vertices[charInfo.vertexIndex].x;

                        if (this.highestX < newHigh) this.highestX = newHigh;
                    }

                    this.diffX = Mathf.Abs(this.highestX - this.lowestX);

                    break;
                }

                default:
                {
                    this.lowestY = this.textInfo.meshInfo[this.textInfo.characterInfo[0].materialReferenceIndex]
                        .vertices[0].y;
                    this.highestY = this.textInfo.meshInfo[this.textInfo.characterInfo[1].materialReferenceIndex]
                        .vertices[4].y;

                    for (int i = 1; i < this.textInfo.characterCount; i++)
                    {
                        TMP_CharacterInfo charInfo = this.textInfo.characterInfo[i];
                        if (!charInfo.isVisible)
                            continue;
                        float newLow = this.textInfo.meshInfo[charInfo.materialReferenceIndex]
                            .vertices[charInfo.vertexIndex].y;

                        if (this.lowestY > newLow) this.lowestY = newLow;
                    }

                    for (int i = 1; i < this.textInfo.characterCount; i++)
                    {
                        TMP_CharacterInfo charInfo = this.textInfo.characterInfo[i];
                        if (!charInfo.isVisible)
                            continue;
                        float newHigh = this.textInfo.meshInfo[charInfo.materialReferenceIndex]
                            .vertices[charInfo.vertexIndex].y;

                        if (this.highestY < newHigh) this.highestY = newHigh;
                    }

                    this.diffY = Mathf.Abs(this.highestY - this.lowestY);

                    break;
                }
            }
        }

        #endregion
    }
}