using UnityEngine;

namespace Runtime.VFX.Profiles
{
    [CreateAssetMenu(menuName = "VFX/Profiles/Just PlayPause")]
    public sealed class JustPlayPauseProfile : EffectProfile
    {
        public override void ObjectStart(ParticleSystem[] particleSystems) =>
            this.Shutdown(particleSystems);

        public override void Startup(ParticleSystem[] particleSystems)
        {
            foreach (ParticleSystem particleSystem in particleSystems) 
                particleSystem.Play();
        }

        public override void Shutdown(ParticleSystem[] particleSystems)
        {
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.Pause();
                particleSystem.Clear();
            }
        }
    }
}
