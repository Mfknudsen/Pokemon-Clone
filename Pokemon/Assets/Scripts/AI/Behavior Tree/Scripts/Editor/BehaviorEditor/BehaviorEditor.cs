#region SDK

using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Math;
using AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter;
//Custom
using AI.BehaviourTreeEditor.EditorNodes;
using AI.BehaviorTree.Nodes;

#endregion

namespace AI.BehaviourTreeEditor
{
    public class BehaviorEditor : EditorWindow
    {
        #region Values

        public static EditorSettings settings;
        public static BehaviorEditor editor;
        public static bool forceSetDirty = true;

        //Header

        //Inspector
        bool showInspector = false;
        BaseNodeSetting lastSelected = null;

        //Window
        Vector3 mousePosition;
        bool clickedOnWindow;
        BaseNodeSetting selectedNode;
        int transitFromId;
        Rect mouseRect = new Rect(0, 0, 1, 1);
        Rect all = new Rect(-5, -5, 10000, 10000);
        GUIStyle style;
        GUIStyle activeStyle;
        Vector2 scrollPos;
        Vector2 scrollStartPos;
        int nodesToDelete;

        //Transition
        BaseNodeSetting drawTrans;

        private enum UserActions
        {
            //Input
            addIntInput,
            addFloatInput,
            addStringInput,
            addVec2Input,
            addVec3Input,
            addTransformInput,

            // -- Pokemon
            addPokeTeamInput,

            //Filler
            addMathClampFiller,
            addRotateFiller,
            addTransSplitFiller,
            addVec2SplitFiller,
            addVec3SplitFiller,

            //Leaf
            addDebugLeaf,

            deleteNode,
            resetPan,
        }

        #endregion

        #region Init

        [MenuItem("Behavior Editor/Editor")]
        static void ShowEditor()
        {
            editor = (BehaviorEditor) GetWindow((typeof(BehaviorEditor)), false, "Behaviour Editor");
            editor.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            if (editor == null)
                editor = this;
            settings = Resources.Load("EditorSettings") as EditorSettings;

            style = settings.skin.GetStyle("window");

            activeStyle = settings.activeSkin.GetStyle("window");
        }

        #endregion

        private void OnFocus()
        {
            if (settings.currentGraph != null)
            {
                if (!settings.currentGraph.behaviour.HasRoot())
                    settings.AddNodeOnGraph(settings.rootNode, new RootNode(), 50, 50, "Root",
                        new Vector2(Screen.width / 2, Screen.height / 2));

                foreach (BaseNodeSetting node in settings.currentGraph.windows.Where(node => node.baseNode == null))
                    node.baseNode = settings.currentGraph.behaviour.GetNodeByID(node.id);

                foreach (BaseNodeSetting node in settings.currentGraph.windows.Where(node =>
                    node.drawNode is DrawTransitionNode))
                {
                    if (node.enterDraw == null)
                    {
                        foreach (BaseNodeSetting bSet in settings.currentGraph.windows.Where(set =>
                            set.id == node.enterID))
                        {
                            node.enterDraw = bSet;
                        }
                    }

                    if (node.exitDraw == null)
                    {
                        foreach (BaseNodeSetting bSet in settings.currentGraph.windows.Where(set =>
                            set.id == node.exitID))
                        {
                            node.exitDraw = bSet;
                        }
                    }
                }
            }
        }

        private void Update()
        {
            if (drawTrans != null)
            {
                drawTrans.mouse = mousePosition;
                if (settings.currentGraph.behaviour.nodes.Count == 0)
                {
                    drawTrans = null;
                }
            }

            if (nodesToDelete > 0)
            {
                if (settings.currentGraph != null)
                {
                    settings.currentGraph.DeleteWindowsThatNeedTo();
                }

                nodesToDelete = 0;
            }

            Repaint();
        }

        private void OnGUI()
        {
            Event e = Event.current;
            mousePosition = e.mousePosition;

            if (mousePosition.x > 0 && mousePosition.y > 0 &&
                mousePosition.x < Screen.width && mousePosition.y < Screen.height)
                UserInput(e);

            DrawNodeView();

            if (e.type == EventType.MouseDrag)
            {
                if (settings.currentGraph != null)
                {
                    //settings.currentGraph.DeleteWindowsThatNeedTo();
                    Repaint();
                }
            }

            if (GUI.changed)
            {
                settings.currentGraph.DeleteWindowsThatNeedTo();
                Repaint();
            }

            /*
            if (settings.makeTransition)
            {
                mouseRect.x = mousePosition.x;
                mouseRect.y = mousePosition.y;
                Rect from = settings.currentGraph.GetNodeWithIndex(transitFromId).windowRect;
                DrawNodeCurve(from, mouseRect, true, Color.blue);
                Repaint();
            }
            */

            if (forceSetDirty)
            {
                forceSetDirty = false;
                EditorUtility.SetDirty(settings);
                EditorUtility.SetDirty(settings.currentGraph);
                EditorUtility.SetDirty(settings.currentGraph.behaviour);

                for (int i = 0; i < settings.currentGraph.windows.Count; i++)
                {
                    BaseNodeSetting n = settings.currentGraph.windows[i];
                }
            }

            EditorGUILayout.BeginVertical();
            Header();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Inspector();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        #region Header

        private void Header()
        {
            EditorGUILayout.BeginHorizontal("box");
            BehaviorGraph graph = (BehaviorGraph) EditorGUILayout.ObjectField(settings.currentGraph,
                typeof(BehaviorGraph), false, GUILayout.Width(200));

            if (settings.currentGraph != graph)
            {
                settings.currentGraph = graph;
            }

            if (graph != null)
            {
                EditorGUILayout.LabelField("Graph Name: ", GUILayout.Width(80));
                string newName = EditorGUILayout.TextField(graph.name, GUILayout.Width(250));

                if (!graph.name.Equals(newName))
                {
                    string assetPath = AssetDatabase.GetAssetPath(graph.GetInstanceID());
                    AssetDatabase.RenameAsset(assetPath, newName);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Inspector

        private void Inspector()
        {
            return;
            Debug.Log((settings.currentGraph != null) + "  :  " + (lastSelected != null));
            if (settings.currentGraph != null && lastSelected != null)
            {
                EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(350));

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIStyle style = GUI.skin.box;
                style.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField("Inspector", GUILayout.Width(style.CalcSize(new GUIContent("Inspector")).x));
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name: ", GUILayout.Width(style.CalcSize(new GUIContent("Name: ")).x));
                selectedNode.windowTitle = EditorGUILayout.TextField(selectedNode.windowTitle);
                EditorGUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }
        }

        #endregion

        #region Node Window

        private void DrawNodeView()
        {
            GUILayout.BeginArea(all, style);

            BeginWindows();

            if (settings.currentGraph != null)
            {
                foreach (BaseNodeSetting n in settings.currentGraph.windows)
                {
                    if (n.baseNode == null)
                        n.baseNode = settings.currentGraph.behaviour.GetNodeByID(n.id);

                    n.DrawCurve(
                        settings.currentGraph.behaviour.GetNodeByID(
                            n.id
                        )
                    );
                }

                foreach (BaseNodeSetting setting in settings.currentGraph.windows)
                {
                    Transition t = setting.baseNode as Transition;

                    if (t == null)
                        continue;
                }

                for (int i = 0; i < settings.currentGraph.windows.Count; i++)
                {
                    BaseNodeSetting b = settings.currentGraph.windows[i];

                    b.windowRect = GUI.Window(i, b.windowRect,
                        DrawNodeWindow, b.windowTitle);
                }
            }

            EndWindows();

            GUILayout.EndArea();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void DrawNodeWindow(int id)
        {
            settings.currentGraph.windows[id].DrawWindow(
                settings.currentGraph.behaviour.GetNodeByID(id)
            );
            GUI.DragWindow();
        }

        private void UserInput(Event e)
        {
            if (settings.currentGraph == null)
                return;

            if (e.button == 1)
                RightClick(e);

            if (e.button == 2)
            {
                if (e.type == EventType.MouseDown)
                {
                    scrollStartPos = e.mousePosition;
                }
                else if (e.type == EventType.MouseDrag)
                {
                    HandlePanning(e);
                }
                else if (e.type == EventType.MouseUp)
                {
                }
            }
        }

        private void HandlePanning(Event e)
        {
            Vector2 diff = e.mousePosition - scrollStartPos;
            diff *= .6f;
            scrollStartPos = e.mousePosition;
            scrollPos += diff;

            for (int i = 0; i < settings.currentGraph.windows.Count; i++)
            {
                BaseNodeSetting b = settings.currentGraph.windows[i];
                b.windowRect.x += diff.x;
                b.windowRect.y += diff.y;
            }
        }

        private void ResetScroll()
        {
            for (int i = 0; i < settings.currentGraph.windows.Count; i++)
            {
                BaseNodeSetting b = settings.currentGraph.windows[i];
                b.windowRect.x -= scrollPos.x;
                b.windowRect.y -= scrollPos.y;
            }

            scrollPos = Vector2.zero;
        }

        private void RightClick(Event e)
        {
            clickedOnWindow = false;
            for (int i = 0; i < settings.currentGraph.windows.Count; i++)
            {
                if (settings.currentGraph.windows[i].windowRect.Contains(e.mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = settings.currentGraph.windows[i];
                    lastSelected = selectedNode;
                    break;
                }
            }

            if (!clickedOnWindow)
            {
                AddNewNode(e);
            }
            else
            {
                ModifyNode(e);
            }
        }

        public void MakeTransition(BaseNodeSetting setting, int infoID, Vector2 pos, bool isTarget)
        {
            BaseNode node = setting.baseNode;

            if (drawTrans == null)
            {
                drawTrans = settings.AddNodeOnGraph(settings.transitionNode, new Transition(), 20, 20, "",
                    Vector2.zero);
                Transition transition = drawTrans.baseNode as Transition;
                transition?.Set(node, infoID, isTarget);

                if (isTarget)
                {
                    drawTrans.enterDraw = setting;
                    drawTrans.preEnterPos = setting.windowRect.position;
                    drawTrans.enterStart = pos;
                    drawTrans.enterID = setting.id;
                }
                else
                {
                    drawTrans.exitDraw = setting;
                    drawTrans.preExitPos = setting.windowRect.position;
                    drawTrans.exitStart = pos;
                    drawTrans.exitID = setting.id;
                }
            }
            else
            {
                if (drawTrans.baseNode == null)
                {
                    if (settings.currentGraph.windows.Contains(drawTrans))
                        settings.currentGraph.windows.Remove(drawTrans);
                    drawTrans = null;
                    return;
                }

                Transition transition = (Transition) drawTrans.baseNode;

                transition.Set(node, infoID, isTarget);

                if (isTarget)
                {
                    drawTrans.enterDraw = setting;
                    drawTrans.preEnterPos = setting.windowRect.position;
                    drawTrans.enterStart = pos;
                    drawTrans.enterID = setting.id;
                }
                else
                {
                    drawTrans.exitDraw = setting;
                    drawTrans.preExitPos = setting.windowRect.position;
                    drawTrans.exitStart = pos;
                    drawTrans.enterID = setting.id;
                }

                if (transition.targetNodeID != -1 && transition.fromNodeID != -1)
                {
                    drawTrans.AddTransitionID(transition.fromNodeID);

                    foreach (BaseNode toSet in settings.currentGraph.behaviour.nodes.Where(n =>
                        n.id == transition.fromNodeID))
                        toSet.AddTransition(transition);

                    drawTrans = null;
                }
            }
        }

        #endregion

        #region Context Menus

        private void AddNewNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");
            if (settings.currentGraph != null)
            {
                #region Input

                menu.AddItem(new GUIContent("Add Input/Int"), false, ContextCallback, UserActions.addIntInput);
                menu.AddItem(new GUIContent("Add Input/Float"), false, ContextCallback, UserActions.addFloatInput);
                menu.AddItem(new GUIContent("Add Input/String"), false, ContextCallback, UserActions.addStringInput);
                menu.AddItem(new GUIContent("Add Input/Vector2"), false, ContextCallback, UserActions.addVec2Input);
                menu.AddItem(new GUIContent("Add Input/Vector3"), false, ContextCallback, UserActions.addVec3Input);
                menu.AddItem(new GUIContent("Add Input/Transform"), false, ContextCallback,
                    UserActions.addTransformInput);
                // -- Pokemon
                menu.AddItem(new GUIContent("Add Input/Pokemon/Team"), false, ContextCallback,
                    UserActions.addPokeTeamInput);

                #endregion

                #region Filler

                //Math
                menu.AddItem(new GUIContent("Add Filler/Math/Clamp"), false, ContextCallback,
                    UserActions.addMathClampFiller);
                //Transform
                menu.AddItem(new GUIContent("Add Filler/Transform/Rotate"), false, ContextCallback,
                    UserActions.addRotateFiller);
                //Splitter
                menu.AddItem(new GUIContent("Add Filler/Split/Vector2 Split"), false, ContextCallback,
                    UserActions.addVec2SplitFiller);
                menu.AddItem(new GUIContent("Add Filler/Split/Vector3 Split"), false, ContextCallback,
                    UserActions.addVec3SplitFiller);
                menu.AddItem(new GUIContent("Add Filler/Split/Transform Split"), false, ContextCallback,
                    UserActions.addTransSplitFiller);

                #endregion

                #region Leaf

                menu.AddItem(new GUIContent("Add Leaf/Debug"), false, ContextCallback, UserActions.addDebugLeaf);

                #endregion

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Reset Panning"), false, ContextCallback, UserActions.resetPan);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Add State"));
                menu.AddDisabledItem(new GUIContent("Add Comment"));
            }

            menu.ShowAsContext();
            e.Use();
        }

        private void ModifyNode(Event e)
        {
            GenericMenu menu = new GenericMenu();

            if (selectedNode.drawNode != null)
            {
                if (selectedNode.isDuplicate || !selectedNode.isAssigned)
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("Make Transition"));
                }
                else
                {
                    menu.AddSeparator("");
                }

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.deleteNode);
            }

            menu.ShowAsContext();
            e.Use();
        }

        private void ContextCallback(object o)
        {
            UserActions a = (UserActions) o;
            switch (a)
            {
                #region Inputs

                case UserActions.addIntInput:
                    settings.AddNodeOnGraph(settings.inputNode, new GetIntNode(), 215, 25, "Int Input", mousePosition);
                    break;
                case UserActions.addVec3Input:
                    settings.AddNodeOnGraph(settings.inputNode, new GetVec3Node(), 215, 25, "Vector3 Input",
                        mousePosition);
                    break;
                case UserActions.addTransformInput:
                    settings.AddNodeOnGraph(settings.inputNode, new GetTransformNode(), 215, 25, "Transform Input",
                        mousePosition);
                    break;
                case UserActions.addFloatInput:
                    settings.AddNodeOnGraph(settings.inputNode, new GetFloatNode(), 215, 25, "Float Input",
                        mousePosition);
                    break;
                case UserActions.addVec2Input:
                    settings.AddNodeOnGraph(settings.inputNode, new GetVec2Node(), 215, 25, "Vector2 Input",
                        mousePosition);
                    break;

                // -- Pokemon
                case UserActions.addPokeTeamInput:
                    settings.AddNodeOnGraph(settings.inputNode, new GetPokeTeamNode(), 215, 25, "Poké Team Input",
                        mousePosition);
                    break;

                #endregion

                #region Filler

                case UserActions.addMathClampFiller:
                    settings.AddNodeOnGraph(settings.fillerNode, new ClampNode(), 125, 25, "Clamp Filler",
                        mousePosition);
                    break;
                case UserActions.addRotateFiller:
                    settings.AddNodeOnGraph(settings.fillerNode, new RotateNode(), 215, 25, "Rotate",
                        mousePosition);
                    break;
                case UserActions.addVec2SplitFiller:
                    settings.AddNodeOnGraph(settings.fillerNode, new Vector2SplitNode(), 215, 25, "Vector2 Split ",
                        mousePosition);
                    break;
                case UserActions.addVec3SplitFiller:
                    settings.AddNodeOnGraph(settings.fillerNode, new Vector3SplitNode(), 215, 25, "Vector3 Split",
                        mousePosition);
                    break;
                case UserActions.addTransSplitFiller:
                    settings.AddNodeOnGraph(settings.fillerNode, new TransformSplitNode(), 215, 25,
                        "Transform Splitter", mousePosition);
                    break;

                #endregion

                #region Leaf

                case UserActions.addDebugLeaf:
                    settings.AddNodeOnGraph(settings.leafNode, new DebugNode(), 125, 25, "Debug Leaf", mousePosition);
                    break;

                #endregion

                case UserActions.deleteNode:
                    if (selectedNode.drawNode != null)
                    {
                        BaseNodeSetting enterNode = settings.currentGraph.GetNodeWithIndex(selectedNode.enterNode);
                    }

                    nodesToDelete++;
                    settings.currentGraph.DeleteNode(selectedNode.id);
                    break;

                case UserActions.resetPan:
                    ResetScroll();
                    break;

                default:
                    break;
            }

            forceSetDirty = true;
        }

        #endregion

        #region Helper Methods

        public static void DrawNodeCurve(Vector2 start, Vector2 end, bool left, Color curveColor)
        {
            /*
            Vector3 startPos = new Vector3(
                (left) ? start.x + start.width : start.x,
                start.y + (start.height * .5f),
                0);

            Vector3 endPos = new Vector3(end.x + (end.width * .5f), end.y + (end.height * .5f), 0);
            */
            Vector3 startPos = start, endPos = end;

            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Color shadow = new Color(0, 0, 0, 1);
            for (int i = 0; i < 1; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, 4);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 3);
        }

        public static void ClearWindowsFromList(List<BaseNodeSetting> l)
        {
            for (int i = 0; i < l.Count; i++)
            {
                //      if (windows.Contains(l[i]))
                //        windows.Remove(l[i]);
            }
        }

        #endregion
    }
}