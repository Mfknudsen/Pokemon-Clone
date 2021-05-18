#region SDK

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
//Custom
using AI.BehaviorTree.Nodes;

#endregion

namespace AI.BehaviourTreeEditor.EditorNodes
{
    [CreateAssetMenu(menuName = "Behavior Tree/Editor/Input Node")]
    public class DrawInputNode : DrawNode
    {
        public override void DrawCurve(BaseNodeSetting b, BaseNode node)
        {
        }

        public override void DrawWindow(BaseNodeSetting b, BaseNode node)
        {
            b.windowRect.height = 25;
            NodeFunc.DisplayOutputs(b, node, 0);
        }
    }
}