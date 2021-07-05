#region SDK

using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Math
{
    [System.Serializable]
    [Node("Math/Clamp", "Clamp")]
    public class ClampNode : BaseNode
    {
        [InputType("To Check", typeof(float))]
        public float toCheck = 0.0f;

        [InputType("Min", typeof(float)), SerializeReference]
        public float min = 0.0f;

        [InputType("Max", typeof(float)), SerializeReference]
        public float max = 0.0f;

        [OutputType("Result", typeof(float)), SerializeReference]
        public float result = 0.0f;

        public override void Tick(BattleAI setup)
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