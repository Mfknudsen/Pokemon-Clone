#region SDK

using System;
using System.Collections.Generic;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes
{
    [Serializable]
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
        public bool collapse;

        [Header("Transition")] public List<int> allTransitionIDs;

        public int enterID = -1, exitID = -1;
        public Type type;
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

        public void SetDraws(bool isTarget, BaseNodeSetting setting, Vector2 pos)
        {
            if (isTarget)
            {
                enterDraw = setting;
                preEnterPos = setting.windowRect.position;
                enterStart = pos;
                enterID = setting.id;
            }
            else
            {
                exitDraw = setting;
                preExitPos = setting.windowRect.position;
                exitStart = pos;
                exitID = setting.id;
            }
        }
    }
}