using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.BehaviorTree.Nodes;

namespace AI.BehaviourTreeEditor.EditorNodes
{
    [CreateAssetMenu(menuName = "Behaviour Tree/Editor/Root Node")]
    public class DrawRootNode : DrawNode
    {
        public override void DrawCurve(BaseNodeSetting b, BaseNode node)
        {
        }

        public override void DrawWindow(BaseNodeSetting b, BaseNode node)
        {
        }
    }
}