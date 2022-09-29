using UnityEngine;

namespace Runtime.VFX.Profiles
{
    public abstract class EffectProfile : ScriptableObject
    {
        protected bool running;

        public bool IsRunning => this.running;

        public abstract void ObjectStart(ParticleSystem[] particleSystems);
        
        public abstract void Startup(ParticleSystem[] particleSystems);
        public abstract void Shutdown(ParticleSystem[] particleSystems);
    }
}
