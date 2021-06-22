#region SDK

using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Math
{
    [System.Serializable]
    [Node("Math/Clamp", "Clamp")]
    public class ClampNode : BaseNode
    {
        [InputType(VariableType.Float, "To Check")]
        public object toCheck = 0.0f;

        [InputType(VariableType.Float, "Min"), SerializeReference]
        public object min = 0.0f;

        [InputType(VariableType.Float, "Max"), SerializeReference]
        public object max = 0.0f;

        [OutputType(VariableType.Float, "Result"), SerializeReference]
        public object result = 0.0f;

        public override void Tick(BehaviorController setup)
        {
            result = Mathf.Clamp((float) toCheck, (float) min, (float) max);

            ContinueTransitions(setup);
        }
    }
}