using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes
{
    [CreateAssetMenu(menuName = "Behavior Tree/Editor/Filler Node")]
    public class DrawFillerNode : DrawNode
    {
        public override void DrawWindow(BaseNodeSetting b, BaseNode node)
        {
            b.windowRect.height = 25;
            int i = NodeFunc.DisplayCalls(b, node);
            i = NodeFunc.DisplayInputs(b, node, i);
            NodeFunc.DisplayOutputs(b, node, i);
        }

        public override void DrawCurve(BaseNodeSetting b, BaseNode node)
        {
        }
    }
}