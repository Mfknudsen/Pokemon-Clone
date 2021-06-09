#region SDK

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AI.BehaviorTree.Nodes;
using Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor;
using Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes;

#endregion

namespace AI.BehaviourTreeEditor.EditorNodes
{
    [CreateAssetMenu(menuName = "Behaviour Tree/Editor/Transition Node")]
    public class DrawTransitionNode : DrawNode
    {
        public override void DrawWindow(BaseNodeSetting b, BaseNode node)
        {
        }

        public override void DrawCurve(BaseNodeSetting b, BaseNode node)
        {
            if (!(node is Transition transNode) || b == null)
                return;

            if (transNode.fromNodeID != -1 && transNode.targetNodeID != -1 &&
                (b.enterDraw != null && b.exitDraw != null))
            {
                b.windowRect.position = b.exitStart - new Vector2(10, 10) + ((b.enterStart - b.exitStart) / 2);

                b.enterStart -= (b.preEnterPos - b.enterDraw.windowRect.position);
                b.preEnterPos = b.enterDraw.windowRect.position;
                b.exitStart -= (b.preExitPos - b.exitDraw.windowRect.position);
                b.preExitPos = b.exitDraw.windowRect.position;

                BehaviorEditor.DrawNodeCurve(
                    b.exitStart,
                    b.enterStart,
                    (b.enterStart.x < b.exitStart.x),
                    Color.green
                );
            }
            else
            {
                if (transNode.fromNodeID != -1)
                    BehaviorEditor.DrawNodeCurve(
                        b.exitStart,
                        b.mouse,
                        (b.mouse.x < b.exitStart.x),
                        Color.red
                    );
                else if (transNode.targetNodeID != -1)
                    BehaviorEditor.DrawNodeCurve(
                        b.mouse,
                        b.enterStart,
                        (b.enterStart.x < b.mouse.x),
                        Color.red
                    );
            }
        }
    }
}