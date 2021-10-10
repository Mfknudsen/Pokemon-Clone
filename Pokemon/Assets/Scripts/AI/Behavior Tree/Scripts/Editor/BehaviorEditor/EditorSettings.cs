#region SDK

using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes;
using UnityEngine; //Custom

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor
{
    [CreateAssetMenu(menuName = "Behavior Tree/Editor/Settings")]
    public class EditorSettings : ScriptableObject
    {
        #region Values

        public BehaviorGraph currentGraph;

        #region Draw Nodes

        [Header("Nodes"), Space(5)] public DrawTransitionNode transitionNode;
        public DrawInputNode inputNode;
        public DrawFillerNode fillerNode;
        public DrawLeafNode leafNode;

        #endregion

        [Header("Skins"), Space(5)] public GUISkin skin;
        public GUISkin activeSkin;

        #endregion

        public BaseNodeSetting AddNodeOnGraph(DrawNode draw, BaseNode node, float width, float height, string title,
            Vector3 pos)
        {
            if (!currentGraph.behavior.AddNode(node))
                return null;
            if (node == null)
                return null;

            BaseNodeSetting baseSetting = new BaseNodeSetting
            {
                drawNode = draw, windowRect = {width = width, height = height}, windowTitle = title
            };
            baseSetting.windowRect.x = pos.x;
            baseSetting.windowRect.y = pos.y;
            currentGraph.windows.Add(baseSetting);
            baseSetting.id = currentGraph.idCount;
            currentGraph.idCount++;
            baseSetting.baseNode = node;

            node.id = baseSetting.id;

            return baseSetting;
        }
    }
}