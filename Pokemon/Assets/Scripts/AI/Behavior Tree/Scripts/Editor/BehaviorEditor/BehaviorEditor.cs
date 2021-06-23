#region SDK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            AddPokemon,
            AddPokeMove,

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

        private List<NodeCreationEntity> creationList = new List<NodeCreationEntity>();

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

            LoadNodesFromProject();
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

                if (_settings.currentGraph.behaviour.nodes == null) return;

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

        #region Setup

        private void LoadNodesFromProject()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsSubclassOf(typeof(BaseNode)) ||
                        type == typeof(Transition) ||
                        type == typeof(RootNode) ||
                        (type == typeof(InputNode) && !type.IsSubclassOf(typeof(InputNode))) ||
                        (type == typeof(LeafNode) && !type.IsSubclassOf(typeof(LeafNode))))
                        continue;

                    if (type.GetCustomAttribute(typeof(NodeAttribute)) is NodeAttribute nodeAttribute)
                    {
                        if (ContainsEntity(type)) continue;

                        NodeCreationEntity entity = new NodeCreationEntity(nodeAttribute.GetMenuName(),
                            nodeAttribute.GetDisplayName(), type);

                        creationList.Add(entity);
                    }
                    else
                        Debug.LogError("Node Missing NodeAttribute\n" +
                                       "" + type);
                }
            }
        }

        private bool ContainsEntity(Type t)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (NodeCreationEntity n in creationList)
            {
                if (n.GetNodeType() == t)
                    return true;
            }

            return false;
        }

        #endregion

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
            if (!showInspector || _settings.currentGraph == null || lastSelected == null ||
                selectedNode.baseNode == null) return;

            const float maxWidth = 275;

            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.BeginVertical();

            #region Titel

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(maxWidth));

            GUILayout.FlexibleSpace();

            GUIStyle guiStyle = GUI.skin.box;

            guiStyle.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.LabelField("Inspector",
                GUILayout.Width(guiStyle.CalcSize(new GUIContent("Inspector")).x));

            EditorGUILayout.EndHorizontal();

            #endregion

            #region Name

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(maxWidth));
            EditorGUILayout.LabelField("Name: ", GUILayout.Width(guiStyle.CalcSize(new GUIContent("Name: ")).x));
            selectedNode.windowTitle = EditorGUILayout.TextField(selectedNode.windowTitle);
            EditorGUILayout.EndHorizontal();

            #endregion

            #region Comment

            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(maxWidth));
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Comment: ", GUILayout.Width(guiStyle.CalcSize(new GUIContent("Comment: ")).x));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            selectedNode.comment = EditorGUILayout.TextField(selectedNode.comment, GUILayout.Height(100));
            EditorGUILayout.EndVertical();

            #endregion

            #region Values

            FieldInfo[] fields = selectedNode.baseNode.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            List<FieldInfo> outputs = new List<FieldInfo>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (FieldInfo f in fields)
            {
                OutputType attribute = (OutputType) Attribute.GetCustomAttribute(f, typeof(OutputType));

                if (attribute == null) continue;

                outputs.Add(f);
            }

            if (outputs.Count > 0)
            {
                GUILayout.Space(20);
                GUILayout.Label("Inputs");

                EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(maxWidth));

                foreach (FieldInfo output in outputs)
                {
                    OutputType attribute = Attribute.GetCustomAttribute(output, typeof(OutputType)) as OutputType;

                    if (attribute == null)
                        continue;
                    if (attribute.varType == VariableType.Default)
                        continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(attribute.name,
                        GUILayout.MaxWidth(style.CalcSize(new GUIContent(attribute.name)).x));

                    object obj = output.GetValue(selectedNode.baseNode);

                    GUILayout.FlexibleSpace();

                    object newValue = EditorMethods.InputField(attribute.varType, obj, attribute.scriptType);

                    if (obj != newValue)
                    {
                        output.SetValue(
                            selectedNode.baseNode,
                            newValue
                        );
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }

            #endregion

            #region Settings

            GUILayout.Space(10);
            GUILayout.Label("Settings");
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(maxWidth));

            //Titel
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Title", GUILayout.Width(style.CalcSize(new GUIContent("Title")).x));
            GUILayout.FlexibleSpace();
            selectedNode.windowTitle = EditorGUILayout.TextField(selectedNode.windowTitle);
            EditorGUILayout.EndHorizontal();

            //Position
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Position", GUILayout.Width(style.CalcSize(new GUIContent("Position")).x));
            GUILayout.FlexibleSpace();
            selectedNode.windowRect.position = EditorGUILayout.Vector2Field("", selectedNode.windowRect.position);
            EditorGUILayout.EndHorizontal();

            //Scale
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Position", GUILayout.Width(style.CalcSize(new GUIContent("Position")).x));
            GUILayout.FlexibleSpace();
            selectedNode.windowRect.size = EditorGUILayout.Vector2Field("", selectedNode.windowRect.size);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            #endregion

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
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

        public void MakeInformationTransition(BaseNodeSetting setting, int infoID, int varID, Vector2 pos,
            bool isTarget)
        {
            Transition t = (Transition) drawTrans?.baseNode;
            if (t is {transferInformation: true})
            {
                if (_settings.currentGraph.windows.Contains(drawTrans))
                    _settings.currentGraph.DeleteNode(drawTrans.id);
                drawTrans = null;
            }

            BaseNode node = setting.baseNode;
            if (drawTrans == null)
            {
                Transition transition = new Transition(true);
                drawTrans = _settings.AddNodeOnGraph(_settings.transitionNode, transition, 20, 20, "",
                    Vector2.zero);
                transition?.Set(node, infoID, isTarget);
                drawTrans.varID = varID;
                drawTrans.SetDraws(isTarget, setting, pos);
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

                if ((isTarget && drawTrans.enterDraw != null) || (!isTarget && drawTrans.exitDraw != null) ||
                    varID != drawTrans.varID || setting.id == drawTrans.enterID || setting.id == drawTrans.exitID)
                {
                    if (_settings.currentGraph.windows.Contains(drawTrans))
                        _settings.currentGraph.DeleteNode(drawTrans.id);
                    drawTrans = null;
                    return;
                }

                Transition transition = (Transition) drawTrans.baseNode;

                transition.Set(node, infoID, isTarget);

                drawTrans.SetDraws(isTarget, setting, pos);

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

        public void MakeActionTransition(BaseNodeSetting setting, bool isTarget, Vector2 pos)
        {
            if (drawTrans != null)
            {
                if (_settings.currentGraph.windows.Contains(drawTrans))
                    _settings.currentGraph.DeleteNode(drawTrans.id);
                drawTrans = null;
            }

            if (drawTrans == null)
            {
                Transition transition = new Transition(false);

                drawTrans = _settings.AddNodeOnGraph(_settings.transitionNode, transition, 20, 20, "",
                    pos);

                BaseNode node = setting.baseNode;

                transition.Set(node, isTarget);

                drawTrans.SetDraws(isTarget, setting, pos);
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

                if ((isTarget && drawTrans.enterDraw != null) || (!isTarget && drawTrans.exitDraw != null) ||
                    setting.id == drawTrans.enterID || setting.id == drawTrans.exitID)
                {
                    if (_settings.currentGraph.windows.Contains(drawTrans))
                        _settings.currentGraph.DeleteNode(drawTrans.id);
                    drawTrans = null;
                    return;
                }

                Transition transition = (Transition) drawTrans.baseNode;

                transition.Set(setting.baseNode, isTarget);

                drawTrans.SetDraws(isTarget, setting, pos);

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
            // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
            if (_settings.currentGraph != null)
            {
                foreach (NodeCreationEntity nodeCreationEntity in creationList)
                {
                    menu.AddItem(new GUIContent(nodeCreationEntity.GetMenuName()), false, ContextCallback,
                        nodeCreationEntity);
                }

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Reset Panning"), false, ContextCallback, true);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Add State"));
                menu.AddDisabledItem(new GUIContent("Add Comment"));
            }

            menu.AddSeparator("");
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
            NodeCreationEntity nodeEntity = (NodeCreationEntity) o;

            if (nodeEntity.GetMenuName() != "")
            {
                BaseNode node = Activator.CreateInstance(nodeEntity.GetNodeType()) as BaseNode;
                DrawNode drawNode;

                if (nodeEntity.GetNodeType().IsSubclassOf(typeof(InputNode)))
                    drawNode = _settings.inputNode;
                else if (nodeEntity.GetNodeType().IsSubclassOf(typeof(LeafNode)))
                    drawNode = _settings.leafNode;
                else
                    drawNode = _settings.fillerNode;


                _settings.AddNodeOnGraph(drawNode, node, 215, 25, nodeEntity.GetDisplayName(),
                    mousePosition);
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

    public readonly struct NodeCreationEntity
    {
        private readonly string menuName, displayName;
        private readonly Type nodeType;

        public NodeCreationEntity(string menuName, string displayName, Type nodeType)
        {
            this.menuName = menuName;
            this.displayName = displayName;
            this.nodeType = nodeType;
        }

        public string GetMenuName()
        {
            return menuName;
        }

        public string GetDisplayName()
        {
            return displayName;
        }

        public Type GetNodeType()
        {
            return nodeType;
        }
    }
}