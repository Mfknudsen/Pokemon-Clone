#region Packages

using NodeCanvas.Framework;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Node_Canvas.Conditions
{
    public class TargetIsPlayer : ConditionTask
    {
        public BBParameter<GameObject> target;

        protected override bool OnCheck()
        {
            return target.value.transform.root.name.Equals("Player");
        }
    }
}