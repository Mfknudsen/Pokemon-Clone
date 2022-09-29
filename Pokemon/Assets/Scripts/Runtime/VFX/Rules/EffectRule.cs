using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.VFX.Rules
{
    [Serializable, ShowOdinSerializedPropertiesInInspector]
    public abstract class EffectRule : object
    {
        public virtual void Enable()  { }
        public virtual void Disable() { }

        public abstract bool CheckRule(GameObject effect);
    }
}
