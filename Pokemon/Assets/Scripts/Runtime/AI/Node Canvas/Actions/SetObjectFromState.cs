#region Packages

using NodeCanvas.Framework;
using UnityEngine;

#endregion

namespace Runtime.AI.Node_Canvas.Actions
{
    public class SetObjectFromState<T> : ActionTask<NpcController> where T : Object
    {
        public BBParameter<string> key;
        public BBParameter<T> toSet;

        protected override void OnExecute()
        {
            object o = agent.GetStateByKey(key.GetValue());

            if (o is T t)
            {
                toSet.SetValue(t);

                EndAction(true);
            }

            EndAction(false);
        }
    }
}