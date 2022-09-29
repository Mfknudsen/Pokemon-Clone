using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.AI;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.VFX.Rules
{
    [Serializable]
    public sealed class ActorDistanceRule : EffectRule
    {
        [BoxGroup(" ", ShowLabel = false)] [SerializeField, BoxGroup(" /Actor Distance")]
        private float maxDistance;

        [SerializeField, BoxGroup(" /Actor Distance")]
        private int minActorCount;

        private List<UnitBase> currentActors = new();

        public override void Enable() => this.currentActors.AddRange(Object.FindObjectsOfType<UnitBase>());

        public override bool CheckRule(GameObject effect)
        {
            Vector3 effectPos = effect.transform.position;

            int cur = this.currentActors
                .Count(actor => Vector3.Distance(effectPos, actor.transform.position) < this.maxDistance);

            return cur >= this.minActorCount;
        }
    }
}