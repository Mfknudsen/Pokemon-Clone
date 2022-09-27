#region Packages

using System.Collections;
using System.Linq;
using DG.Tweening;
using Runtime.ScriptableVariables.Structs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

#endregion

namespace Runtime.VFX.Foliage
{
    public sealed class FallingLeafsEffect : EffectBase
    {
        #region Values

        [BoxGroup("Leaf Effect")] [SerializeField, Required]
        private VisualEffect[] particles;

        [BoxGroup("Leaf Effect")] [SerializeField]
        private float time;

        [BoxGroup("Leaf Effect")] [SerializeField, Required]
        private Vec2Variable windDirection;

        #endregion

        #region Internal

        protected override void Enable()
        {
            foreach (VisualEffect effect in this.particles)
            {
                effect.Play();
                Fade(effect, 1, this.time);
            }
        }

        protected override void OnStopDisable() => Enable();

        protected override IEnumerator DisableCoroutine()
        {
            Tweener[] tweeners = new Tweener[this.particles.Length];
            for (int i = 0; i < this.particles.Length; i++)
                tweeners[i] = Fade(this.particles[i], 0, this.time);

            yield return null;

            yield return new WaitWhile(() => tweeners.Any(t => t.active));

            foreach (VisualEffect effect in this.particles)
                effect.Stop();
        }

        #endregion
    }
}