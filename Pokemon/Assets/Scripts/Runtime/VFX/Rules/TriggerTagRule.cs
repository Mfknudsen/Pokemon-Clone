using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.VFX.Rules
{
    [Serializable]
    public class TriggerTagRule : TriggerRule
    {
        [BoxGroup(" ")]
        [SerializeField, BoxGroup(" /Trigger Tag")]
        private string tag;
        [SerializeField, BoxGroup(" /Trigger Tag")]
        private int minCount;

        private int count;

        public override void Add(Collider collider)
        {
            if (collider.tag.Equals(this.tag))
                this.count++;
        }

        public override void Remove(Collider collider)
        {
            if (collider.tag.Equals(this.tag))
                this.count--;
        }

        public override bool CheckRule(GameObject effect) => this.count >= this.minCount;
    }
}
