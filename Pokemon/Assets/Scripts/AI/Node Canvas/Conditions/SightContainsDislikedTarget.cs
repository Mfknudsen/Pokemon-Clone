#region Packages

using System.Collections.Generic;
using Mfknudsen.AI.Senses;
using NodeCanvas.Framework;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Node_Canvas.Conditions
{
    public class SightContainsDislikedTarget : ConditionTask
    {
        public BBParameter<SightLine> sight;
        public BBParameter<List<string>> disliked;

        protected override bool OnCheck()
        {
            foreach (GameObject gameObject in sight.value.GetInSight())
            {
                if (disliked.value.Contains(gameObject.name))
                    return true;
            }

            return false;
        }
    }
}