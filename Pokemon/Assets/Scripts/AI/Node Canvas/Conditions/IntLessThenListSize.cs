#region Packages

using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Node_Canvas.Conditions
{
    public class IntLessThenListSize : ConditionTask
    {
        public BBParameter<int> index;
        public BBParameter<List<Transform>> list;

        protected override bool OnCheck()
        {
            return index.value < list.value.Count;
        }
    }
}