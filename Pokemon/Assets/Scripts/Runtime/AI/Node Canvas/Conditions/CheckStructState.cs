#region Packages

using NodeCanvas.Framework;

#endregion

namespace Runtime.AI.Node_Canvas.Conditions
{
    public class CheckStructState<T> : ConditionTask where T : struct
    {
        public BBParameter<string> key;
        public BBParameter<T> checkAgainst;

        protected override bool OnCheck()
        {
            return checkAgainst.value.Equals(agent.GetComponent<NpcController>().GetStateByKey(key.GetValue()));
        }
    }
}
