using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.VFX.Rules
{
    [Serializable]
    public sealed class DistanceRule : EffectRule
    {
        [BoxGroup(" ", ShowLabel = false)]
        [SerializeField, BoxGroup(" /Distance")] private float maxDistance;
        [SerializeField, BoxGroup(" /Distance")] private Transform target;
        
        public override bool CheckRule(GameObject effect) => 
            Vector3.Distance(this.target.position, effect.transform.position) <= this.maxDistance;
    }
}
