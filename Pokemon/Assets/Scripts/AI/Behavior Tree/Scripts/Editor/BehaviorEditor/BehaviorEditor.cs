#region SDK

using System;
using System.Linq;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Math;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input.PokemonNodes;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Leaf;
using Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Screen;
using EditorSettings = Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.EditorSettings;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor
{
    public class BehaviorEditor : EditorWindow
    {
        #region Values

        private static EditorSettings _settings;
        public static BehaviorEditor editor;
        private static bool _forceSetDirty = true;

        //Header

        //Inspector
        public KeyCode inspectorKey = KeyCode.Tab;
        private bool showInspector;
        private BaseNodeSetting lastSelected;

        //Window
        private bool hasWindowFocus = true;
        private Vector3 mousePosition;
        private bool clickedOnWindow;
        private BaseNodeSetting selectedNode;
        private readonly Rect all = new Rect(-5, -5, 10000, 10000);
        private GUIStyle style;
        private Vector2 scrollPos;
        private Vector2 scrollStartPos;
        private int nodesToDelete;

        //Transition
        private BaseNodeSetting drawTrans;

        private enum UserActions
        {
            //Input
            AddIntInput,
            AddFloatInput,
            AddStringInput,
            AddVec2Input,
            AddVec3Input,
            AddTransformInput,

            // -- Pokemon
            AddPokeTeamInput,

            //Filler
            AddMathClampFiller,
            AddRotateFiller,
            AddTransSplitFiller,
            AddVec2SplitFiller,
            AddVec3SplitFiller,

            //Leaf
            AddDebugLeaf,

            DeleteNode,
            ResetPan,
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
            _settings = Resources.Load("EditorSettings") as EditorSettings;

            if (_settings is null) return;

            style = _settings.skin.GetStyle("window");
        }

        #endregion

        private void OnFocus()
        {
            hasWindowFocus = true;

            if (_settings == null) return;
            if (_settings.currentGraph == null) return;

            foreach (BaseNodeSetting node in _settings.currentGraph.windows.Where(node => node.baseNode == null))
                node.baseNode = _settings.currentGraph.behaviour.GetNodeByID(node.id);

            foreach (BaseNodeSetting node in _settings.currentGraph.windows.Where(node =>
                node.drawNode is DrawTransitionNode))
            {
                if (node.enterDraw == null)
                {
                    foreach (BaseNodeSetting bSet in _settings.currentGraph.windows.Where(set =>
                        set.id == node.enterID))
                    {
                        node.enterDraw = bSet;
                    }
                }

                if (node.exitDraw == null)
                {
                    foreach (BaseNodeSetting bSet in _settings.currentGraph.windows.Where(set =>
                        set.id == node.exitID))
                    {
                        node.exitDraw = bSet;
                    }
                }
            }
        }

        private void OnLostFocus()
        {
            hasWindowFocus = false;
        }

        private void Update()
        {
            if (drawTrans != null)
            {
                drawTrans.mouse = mousePosition;
                
                if(_settings.currentGraph.behaviour.nodes == null) return;
                
                if (_settings.currentGraph.behaviour.nodes.Count == 0)
                {
                    drawTrans = null;
                }
            }

            if (nodesToDelete > 0)
            {
                if (_settings.currentGraph != null)
                {
                    _settings.currentGraph.DeleteWindowsThatNeedTo();
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
                mousePosition.x < width && mousePosition.y < height)
                UserInput(e);

            DrawNodeView();

            if (e.type == EventType.MouseDrag)
            {
                if (_settings.currentGraph != null)
                {
                    //settings.currentGraph.DeleteWindowsThatNeedTo();
                    Repaint();
                }
            }

            if (GUI.changed)
            {
                _settings.currentGraph.DeleteWindowsThatNeedTo();
                Repaint();
            }

            if (_forceSetDirty)
            {
                _forceSetDirty = false;
                EditorUtility.SetDirty(_settings);
                EditorUtility.SetDirty(_settings.currentGraph);
                EditorUtility.SetDirty(_settings.currentGraph.behaviour);
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
            BehaviorGraph graph = (BehaviorGraph) EditorGUILayout.ObjectField(_settings.currentGraph,
                typeof(BehaviorGraph), false, GUILayout.Width(200));

            _settings.currentGraph = graph;

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
            if (!showInspector || _settings.currentGraph == null || lastSelected == null) return;

            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(350));
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle guiStyle = GUI.skin.box;
            guiStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("Inspector",
                GUILayout.Width(guiStyle.CalcSize(new GUIContent("Inspector")).x));
            EditorGUILayout.EndHorizontal();

            //Name
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name: ", GUILayout.Width(guiStyle.CalcSize(new GUIContent("Name: ")).x));
            selectedNode.windowTitle = EditorGUILayout.TextField(selectedNode.windowTitle);
            EditorGUILayout.EndHorizontal();

            //Comment
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Comment: ", GUILayout.Width(guiStyle.CalcSize(new GUIContent("Comment: ")).x));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            selectedNode.comment = EditorGUILayout.TextField(selectedNode.comment, GUILayout.Height(100));
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Node Window

        private void DrawNodeView()
        {
            GUILayout.BeginArea(all, style);

            BeginWindows();

            if (_settings.currentGraph != null)
            {
                foreach (BaseNodeSetting n in _settings.currentGraph.windows)
                {
                    n.baseNode ??= _settings.currentGraph.behaviour.GetNodeByID(n.id);

                    n.DrawCurve(
                        _settings.currentGraph.behaviour.GetNodeByID(
                            n.id
                        )
                    );
                }

                for (int i = 0; i < _settings.currentGraph.windows.Count; i++)
                {
                    BaseNodeSetting b = _settings.currentGraph.windows[i];

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
            _settings.currentGraph.windows[id].DrawWindow(
                _settings.currentGraph.behaviour.GetNodeByID(id)
            );
            GUI.DragWindow();
        }

        private void UserInput(Event e)
        {
            if (_settings.currentGraph == null || !hasWindowFocus)
                return;

            switch (e.button)
            {
                case 0 when e.type == EventType.MouseDown:
                    foreach (BaseNodeSetting b in _settings.currentGraph.windows.Where(b =>
                        b.windowRect.Contains(mousePosition)))
                    {
                        selectedNode = b;
                        lastSelected = selectedNode;
                        clickedOnWindow = true;
                    }
                    break;
                
                case 1:
                    RightClick(e);
                    break;

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                case 2 when e.type == EventType.MouseDown:
                    scrollStartPos = e.mousePosition;
                    break;

                case 2:
                    if (e.type == EventType.MouseDrag)
                        HandlePanning(e);
                    break;
            }

            // ReSharper disable once InvertIf
            if (e.isKey && e.type == EventType.KeyDown)
            {
                if (e.keyCode == inspectorKey)
                    showInspector = !showInspector;
            }
        }

        private void HandlePanning(Event e)
        {
            Vector2 diff = e.mousePosition - scrollStartPos;
            diff *= .6f;
            scrollStartPos = e.mousePosition;
            scrollPos += diff;

            foreach (BaseNodeSetting b in _settings.currentGraph.windows)
            {
                b.windowRect.x += diff.x;
                b.windowRect.y += diff.y;
            }
        }

        private void ResetScroll()
        {
            foreach (BaseNodeSetting b in _settings.currentGraph.windows)
            {
                b.windowRect.x -= scrollPos.x;
                b.windowRect.y -= scrollPos.y;
            }

            scrollPos = Vector2.zero;
        }

        private void RightClick(Event e)
        {
            clickedOnWindow = false;
            foreach (BaseNodeSetting t in _settings.currentGraph.windows.Where(t =>
                t.windowRect.Contains(e.mousePosition)))
            {
                clickedOnWindow = true;
                selectedNode = t;
                lastSelected = selectedNode;
                break;
            }

            if (!clickedOnWindow)
                AddNewNode(e);
            else
                ModifyNode(e);
        }

        public void MakeTransition(BaseNodeSetting setting, int infoID, int varID, Vector2 pos, bool isTarget)
        {
            BaseNode node = setting.baseNode;
            if (drawTrans == null)
            {
                drawTrans = _settings.AddNodeOnGraph(_settings.transitionNode, new Transition(), 20, 20, "",
                    Vector2.zero);
                Transition transition = drawTrans.baseNode as Transition;
                transition?.Set(node, infoID, isTarget);
                drawTrans.varID = varID;
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
                    if (_settings.currentGraph.windows.Contains(drawTrans))
                        _settings.currentGraph.DeleteNode(drawTrans.id);
                    drawTrans = null;
                    return;
                }

                if ((isTarget && drawTrans.enterDraw != null) ||
                    (!isTarget && drawTrans.exitDraw != null) ||
                    varID != drawTrans.varID ||
                    setting.id == drawTrans.enterID || setting.id == drawTrans.exitID)
                {
                    if (_settings.currentGraph.windows.Contains(drawTrans))
                        _settings.currentGraph.DeleteNode(drawTrans.id);
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
                    drawTrans.exitID = setting.id;
                }

                if (transition.targetNodeID != -1 && transition.fromNodeID != -1)
                {
                    drawTrans.AddTransitionID(transition.fromNodeID);

                    foreach (BaseNode toSet in _settings.currentGraph.behaviour.nodes.Where(n =>
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
            if (_settings.currentGraph != null)
            {
                #region Input

                menu.AddItem(new GUIContent("Add Input/Int"), false, ContextCallback, UserActions.AddIntInput);
                menu.AddItem(new GUIContent("Add Input/Float"), false, ContextCallback, UserActions.AddFloatInput);
                menu.AddItem(new GUIContent("Add Input/String"), false, ContextCallback, UserActions.AddStringInput);
                menu.AddItem(new GUIContent("Add Input/Vector2"), false, ContextCallback, UserActions.AddVec2Input);
                menu.AddItem(new GUIContent("Add Input/Vector3"), false, ContextCallback, UserActions.AddVec3Input);
                menu.AddItem(new GUIContent("Add Input/Transform"), false, ContextCallback,
                    UserActions.AddTransformInput);
                // -- Pokemon
                menu.AddItem(new GUIContent("Add Input/Pokemon/Team"), false, ContextCallback,
                    UserActions.AddPokeTeamInput);

                #endregion

                #region Filler

                //Math
                menu.AddItem(new GUIContent("Add Filler/Math/Clamp"), false, ContextCallback,
                    UserActions.AddMathClampFiller);
                //Transform
                menu.AddItem(new GUIContent("Add Filler/Transform/Rotate"), false, ContextCallback,
                    UserActions.AddRotateFiller);
                //Splitter
                menu.AddItem(new GUIContent("Add Filler/Split/Vector2 Split"), false, ContextCallback,
                    UserActions.AddVec2SplitFiller);
                menu.AddItem(new GUIContent("Add Filler/Split/Vector3 Split"), false, ContextCallback,
                    UserActions.AddVec3SplitFiller);
                menu.AddItem(new GUIContent("Add Filler/Split/Transform Split"), false, ContextCallback,
                    UserActions.AddTransSplitFiller);

                #endregion

                #region Leaf

                menu.AddItem(new GUIContent("Add Leaf/Debug"), false, ContextCallback, UserActions.AddDebugLeaf);

                #endregion

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Reset Panning"), false, ContextCallback, UserActions.ResetPan);
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
                menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.DeleteNode);
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

                case UserActions.AddIntInput:
                    _settings.AddNodeOnGraph(_settings.inputNode, new GetIntNode(), 215, 25, "Int Input",
                        mousePosition);
                    break;
                case UserActions.AddVec3Input:
                    _settings.AddNodeOnGraph(_settings.inputNode, new GetVec3Node(), 215, 25, "Vector3 Input",
                        mousePosition);
                    break;
                case UserActions.AddTransformInput:
                    _settings.AddNodeOnGraph(_settings.inputNode, new GetTransformNode(), 215, 25, "Transform Input",
                        mousePosition);
                    break;
                case UserActions.AddFloatInput:
                    _settings.AddNodeOnGraph(_settings.inputNode, new GetFloatNode(), 215, 25, "Float Input",
                        mousePosition);
                    break;
                case UserActions.AddVec2Input:
                    _settings.AddNodeOnGraph(_settings.inputNode, new GetVec2Node(), 215, 25, "Vector2 Input",
                        mousePosition);
                    break;

                // -- Pokemon
                case UserActions.AddPokeTeamInput:
                    _settings.AddNodeOnGraph(_settings.inputNode, new GetPokeTeamNode(), 215, 25, "Pokémon Team Input",
                        mousePosition);
                    break;

                #endregion

                #region Filler

                case UserActions.AddMathClampFiller:
                    _settings.AddNodeOnGraph(_settings.fillerNode, new ClampNode(), 125, 25, "Clamp Filler",
                        mousePosition);
                    break;
                case UserActions.AddRotateFiller:
                    _settings.AddNodeOnGraph(_settings.fillerNode, new RotateNode(), 215, 25, "Rotate",
                        mousePosition);
                    break;
                case UserActions.AddVec2SplitFiller:
                    _settings.AddNodeOnGraph(_settings.fillerNode, new Vector2SplitNode(), 215, 25, "Vector2 Split ",
                        mousePosition);
                    break;
                case UserActions.AddVec3SplitFiller:
                    _settings.AddNodeOnGraph(_settings.fillerNode, new Vector3SplitNode(), 215, 25, "Vector3 Split",
                        mousePosition);
                    break;
                case UserActions.AddTransSplitFiller:
                    _settings.AddNodeOnGraph(_settings.fillerNode, new TransformSplitNode(), 215, 25,
                        "Transform Splitter", mousePosition);
                    break;

                #endregion

                #region Leaf

                case UserActions.AddDebugLeaf:
                    _settings.AddNodeOnGraph(_settings.leafNode, new DebugNode(), 125, 25, "Debug Leaf", mousePosition);
                    break;

                #endregion

                case UserActions.DeleteNode:
                    if (selectedNode.drawNode == null)
                        break;

                    nodesToDelete++;
                    _settings.currentGraph.DeleteNode(selectedNode.id);
                    break;

                case UserActions.ResetPan:
                    ResetScroll();
                    break;

                default:
                    break;
            }

            _forceSetDirty = true;
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

        #endregion
    }
}