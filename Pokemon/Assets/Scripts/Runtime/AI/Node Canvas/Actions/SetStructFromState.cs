#region Packages

using NodeCanvas.Framework;

#endregion

namespace Runtime.AI.Node_Canvas.Actions
{
    public class SetStructFromState<T> : ActionTask<NpcController> where T : struct
    {
        public BBParameter<string> key;
        public BBParameter<T> toSet;

        protected override void OnExecute()
        {
            object o = agent.GetStateByKey(this.key.GetValue());

            if (o is T t)
            {
                this.toSet.SetValue(t);

                EndAction(true);
            }

            EndAction(false);
        }
    }
}