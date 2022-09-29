using System;
using Runtime.ScriptableVariables.Objects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.VFX.Rules
{
    [Serializable]
    public sealed class PlayerDistanceRule : EffectRule
    {
        [BoxGroup(" ", ShowLabel = false)] [SerializeField, BoxGroup(" /Player Distance"), Required]
        private TransformVariable player;

        [SerializeField, BoxGroup(" /Player Distance")]
        private float maxDistance;

        public override bool CheckRule(GameObject effect)
        {
            if (this.player.value == null)
                return false;

            return Vector3.Distance(
                       effect.transform.position,
                       this.player.value.position)
                   <= this.maxDistance;
        }
    }
}