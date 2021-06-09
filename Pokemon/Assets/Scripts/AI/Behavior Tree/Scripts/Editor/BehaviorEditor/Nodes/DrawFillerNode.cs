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
            int i = NodeFunc.DisplayInputs(b, node, 0);
            NodeFunc.DisplayOutputs(b, node, i);
        }

        public override void DrawCurve(BaseNodeSetting b, BaseNode node)
        {
        }
    }
}