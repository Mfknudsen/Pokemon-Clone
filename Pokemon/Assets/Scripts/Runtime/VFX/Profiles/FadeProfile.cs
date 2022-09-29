using DG.Tweening;
using Runtime.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.VFX.Profiles
{
    [CreateAssetMenu(menuName = "VFX/Profiles/Fade")]
    public sealed class FadeProfile : EffectProfile
    {
        [HorizontalGroup(" ")]
        [SerializeField, BoxGroup(" /Fade In")] private float fadeInTime;
        [SerializeField, BoxGroup(" /Fade Out")] private float fadeOutTime;
        [SerializeField, BoxGroup(" /Fade In")] private AnimationCurve fadeInCurve;
        [SerializeField, BoxGroup(" /Fade Out")] private AnimationCurve fadeOutCurve;
        private Tweener[] currentRunning;

        public override void ObjectStart(ParticleSystem[] particleSystems)
        {
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.Stop();
                particleSystem.Clear();

                if (!(particleSystem.GetComponent<ParticleSystemRenderer>() is { } renderer)) continue;

                foreach (Material material in renderer.materials)
                    material.color *= new Color(1, 1, 1, 0);
            }
        }

        public override void Startup(ParticleSystem[] particleSystems)
        {
            this.running = true;
            if (this.currentRunning != null)
            {
                foreach (Tweener tweener in this.currentRunning)
                    tweener.Kill();
            }

            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.gameObject.SetActive(true);
                particleSystem.Play();

                EffectManager.FadeOverTime(
                    particleSystem,
                    1,
                    this.fadeInTime,
                    false,
                    this.fadeInCurve,
                    () => this.running = false);
            }
        }
        public override void Shutdown(ParticleSystem[] particleSystems)
        {
            this.running = true;
            if (this.currentRunning != null)
            {
                foreach (Tweener tweener in this.currentRunning)
                    tweener.Kill();
            }

            foreach (ParticleSystem particleSystem in particleSystems)
            {
                EffectManager.FadeOverTime(
                    particleSystem,
                    0,
                    this.fadeOutTime,
                    false,
                    this.fadeOutCurve,
                    () =>
                    {
                        particleSystem.Stop();
                        particleSystem.Clear();
                        if (particleSystem.main.stopAction == ParticleSystemStopAction.Disable)
                            particleSystem.gameObject.SetActive(false);
                        this.running = false;
                    });
            }
        }
    }
}
