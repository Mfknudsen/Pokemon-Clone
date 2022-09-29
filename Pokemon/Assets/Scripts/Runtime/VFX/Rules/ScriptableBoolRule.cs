using System;
using Runtime.ScriptableVariables.Structs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.VFX.Rules
{
    [Serializable, ShowOdinSerializedPropertiesInInspector]
    public sealed class ScriptableBoolRule : EffectRule
    {
        [BoxGroup(" ", ShowLabel = false)]
        [SerializeField, BoxGroup(" /Atom Bool"), Required] private BoolVariable variable;
        [SerializeField, BoxGroup(" /Atom Bool")] private bool targetState;

        public override bool CheckRule(GameObject effect) => this.variable != null && this.variable.value == this.targetState;
    }
}
