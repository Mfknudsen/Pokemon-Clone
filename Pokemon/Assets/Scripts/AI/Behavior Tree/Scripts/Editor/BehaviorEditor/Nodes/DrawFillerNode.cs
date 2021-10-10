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
            NodeFunc.DisplayCalls(b, node);
            NodeFunc.DisplayInputs(b, node);
            NodeFunc.DisplayOutputs(b, node);
        }

        public override void DrawCurve(BaseNodeSetting b, BaseNode node)
        {
        }
    }
}