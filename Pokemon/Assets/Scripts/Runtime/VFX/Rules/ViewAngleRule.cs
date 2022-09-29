using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.VFX.Rules
{
    [Serializable, ShowOdinSerializedPropertiesInInspector]
    public sealed class ViewAngleRule : EffectRule
    {
        [BoxGroup(" ", ShowLabel = false)]
        [SerializeField, BoxGroup(" /View Angle")] private float minAngle;
        [SerializeField, BoxGroup(" /View Angle")] private Transform target;
        public override bool CheckRule(GameObject effect) => this.target != null && Vector3.Angle(this.target.forward, effect.transform.position - this.target.position) <= this.minAngle;
    }
}
