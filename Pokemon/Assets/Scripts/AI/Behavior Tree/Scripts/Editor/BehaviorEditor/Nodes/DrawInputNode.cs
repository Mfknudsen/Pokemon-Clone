#region SDK

using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using UnityEngine; //Custom

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes
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