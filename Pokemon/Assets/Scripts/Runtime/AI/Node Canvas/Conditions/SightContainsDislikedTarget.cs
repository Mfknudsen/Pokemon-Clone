#region Packages

using System.Collections.Generic;
using NodeCanvas.Framework;
using Runtime.AI.Senses;
using UnityEngine;

#endregion

namespace Runtime.AI.Node_Canvas.Conditions
{
    public class SightContainsDislikedTarget : ConditionTask
    {
        public BBParameter<Sight> sight;
        public BBParameter<List<string>> disliked;

        protected override bool OnCheck()
        {
            foreach (GameObject gameObject in this.sight.value.GetInSight())
            {
                if (this.disliked.value.Contains(gameObject.name))
                    return true;
            }

            return false;
        }
    }
}