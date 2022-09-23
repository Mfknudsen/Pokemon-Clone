#region Package

using System.Collections;
using DG.Tweening;
using Runtime.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

#endregion

namespace Runtime.VFX
{
    public class EffectBase : MonoBehaviour
    {
        #region Values

        [BoxGroup("Base Effect")] [SerializeField, Required]
        protected EffectManager effectManager;

        [BoxGroup("Base Effect"), Min(0)] [SerializeField]
        private float activeDistanceFromPlayer;

        [BoxGroup("Base Effect")] [SerializeField]
        private bool isStatic;

        private Coroutine coroutine;
        private bool isDisablingSelf;

        #endregion

        #region Build In States

        private void OnEnable()
        {
            this.effectManager.RegisterEffect(this);
            Enable();
            this.coroutine = StartCoroutine(EnableCoroutine());
        }

        private void OnDisable()
        {
            this.effectManager.UnregisterEffect(this);
            Disable();
        }

        #endregion

        #region Getters

        public float getMaxDistance => this.activeDistanceFromPlayer;

        public bool getIsDisablingSelf => this.isDisablingSelf;

        #endregion

        #region In

        public void StartDisable()
        {
            this.isDisablingSelf = true;
            if (this.coroutine != null)
                StopCoroutine(this.coroutine);
            this.coroutine = StartCoroutine(PrivateDisableCoroutine());
        }

        public void StopDisable()
        {
            this.isDisablingSelf = false;
            StopCoroutine(this.coroutine);
            OnStopDisable();
        }

        #endregion

        #region Internal

        protected virtual void Enable()
        {
        }

        protected virtual void Disable()
        {
        }

        protected virtual void OnStopDisable()
        {
        }

        private IEnumerator PrivateDisableCoroutine()
        {
            yield return DisableCoroutine();

            if (!this.isStatic)
                gameObject.SetActive(false);
        }

        protected virtual IEnumerator DisableCoroutine()
        {
            yield break;
        }

        protected virtual IEnumerator EnableCoroutine()
        {
            yield break;
        }

        #region Statics

        protected static Tweener Fade(VisualEffect particleSystem, float endValue, float time)
        {
            return DOTween.To(
                () => particleSystem.GetFloat("FadeAlpha"),
                f => particleSystem.SetFloat("FadeAlpha", f),
                endValue,
                time);
        }

        #endregion

        #endregion
    }
}