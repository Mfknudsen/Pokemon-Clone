using UnityEngine;

namespace Runtime.VFX.Rules
{
    public abstract class TriggerRule : EffectRule
    {
        public abstract void Add(Collider collider);
        public abstract void Remove(Collider collider);
    }
}
