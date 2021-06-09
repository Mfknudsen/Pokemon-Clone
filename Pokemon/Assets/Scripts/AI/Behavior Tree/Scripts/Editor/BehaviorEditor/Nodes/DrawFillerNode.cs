using System.Collections;
using System.Collections.Generic;
using AI.BehaviorTree.Nodes;
using AI.BehaviourTreeEditor.EditorNodes;
using Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes;
using UnityEngine;

namespace AI.BehaviourTreeEditor.EditorNodes
{
    [CreateAssetMenu(menuName = "Behavior Tree/Editor/Filler Node")]
    public class DrawFillerNode : DrawNode
    {
        public override void DrawWindow(BaseNodeSetting b, BaseNode node)
        {
            b.windowRect.height = 25;
            int i = NodeFunc.DisplayInputs(b, node, 0);
            NodeFunc.DisplayOutputs(b, node, i);
        }

        public override void DrawCurve(BaseNodeSetting b, BaseNode node)
        {
        }
    }
}