#region SDK

using System.Collections.Generic;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes
{
    [System.Serializable]
    public class BaseNodeSetting
    {
        //Base
        public int id;
        public DrawNode drawNode;
        public BaseNode baseNode;
        public Rect windowRect;
        public string windowTitle;
        public int enterNode;
        public int targetNode;
        public bool isDuplicate;
        public string comment;
        public bool isAssigned;
        public bool showDescription;
        public bool isOnCurrent;

        public bool collapse;
        public bool showActions = true;
        public bool showEnterExit;
        [HideInInspector] public bool previousCollapse;

        [Header("Transition")] public List<int> allTransitionIDs;

        public int enterID = -1, exitID = -1;
        public int varID = -1;
        public BaseNodeSetting enterDraw, exitDraw;

        public Vector2 preEnterPos,
            preExitPos,
            enterStart,
            exitStart,
            mouse;

        public void DrawWindow(BaseNode node)
        {
            if (drawNode == null)
                return;
            drawNode.DrawWindow(this, node);
        }

        public void DrawCurve(BaseNode node)
        {
            if (drawNode == null)
                return;
            drawNode.DrawCurve(this, node);
        }

        public void AddTransitionID(int transID)
        {
            allTransitionIDs ??= new List<int>();

            allTransitionIDs.Add(transID);
        }
    }
}