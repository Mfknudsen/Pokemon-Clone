using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AI.BehaviorTree.Nodes;

namespace AI.BehaviourTreeEditor.EditorNodes
{
    [CreateAssetMenu(menuName = "Behavior Tree/Editor/Leaf Node")]
    public class DrawLeafNode : DrawNode
    {
        public override void DrawCurve(BaseNodeSetting b, BaseNode node)
        {
        }

        public override void DrawWindow(BaseNodeSetting b, BaseNode node)
        {
            b.windowRect.height = 25;
            NodeFunc.DisplayInputs(b, node, 0);
        }
    }
}