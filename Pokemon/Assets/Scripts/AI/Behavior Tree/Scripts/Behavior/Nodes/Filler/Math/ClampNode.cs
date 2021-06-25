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
        public float toCheck = 0.0f;

        [InputType(VariableType.Float, "Min"), SerializeReference]
        public float min = 0.0f;

        [InputType(VariableType.Float, "Max"), SerializeReference]
        public float max = 0.0f;

        [OutputType(VariableType.Float, "Result"), SerializeReference]
        public float result = 0.0f;

        public override void Tick(BehaviorController setup)
        {
            result = Mathf.Clamp(toCheck, min, max);

            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
            result = 0.0f;
        }
    }
}