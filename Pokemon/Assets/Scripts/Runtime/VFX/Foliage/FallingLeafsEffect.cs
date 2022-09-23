using System.Collections;
using DG.Tweening;
using Runtime.ScriptableVariables.Structs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Runtime.VFX.Foliage
{
    public sealed class FallingLeafsEffect : EffectBase
    {
        #region Values

        [BoxGroup("Leaf Effect")] [SerializeField, Required]
        private VisualEffect particles;

        [BoxGroup("Leaf Effect")] [SerializeField]
        private float time;

        [BoxGroup("Leaf Effect")] [SerializeField, Required]
        private Vec2Variable windDirection;

        #endregion

        #region Internal

        protected override void Enable()
        {
            this.particles.Play();
            Fade(this.particles, 1, this.time);
        }

        protected override void OnStopDisable() => Enable();

        protected override IEnumerator DisableCoroutine()
        {
            Tweener tweener = Fade(this.particles, 0, this.time);

            yield return null;

            yield return new WaitWhile(() => tweener.active);

            this.particles.Stop();
        }

        #endregion
    }
}