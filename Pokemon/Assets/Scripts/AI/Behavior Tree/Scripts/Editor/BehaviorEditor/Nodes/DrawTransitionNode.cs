#region SDK

using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes
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

            Color c = Color.green;
            if (!((Transition) node).transferInformation)
                c = Color.white;

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
                    c
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