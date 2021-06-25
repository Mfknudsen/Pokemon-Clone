﻿using UnityEngine; 

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input
{
    [System.Serializable]
    [Node("Input/Int", "Int Input")]
    public class GetIntNode : InputNode
    {
        [OutputType(VariableType.Int, "Int Input"), SerializeReference]
        public object input = 0;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}