#region SDK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Math;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input;
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
        private bool hasWindowFocus;
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

        private readonly List<NodeCreationEntity> creationList = new List<NodeCreationEntity>();

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
                node.baseNode = _settings.currentGraph.behavior.GetNodeByID(node.id);

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

            CheckRootNode();

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

                if (_settings.currentGraph.behavior.nodes == null) return;

                if (_settings.currentGraph.behavior.nodes.Count == 0)
                {
                    drawTrans = null;
                }
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

            if (_forceSetDirty)
            {
                _forceSetDirty = false;
                EditorUtility.SetDirty(_settings);
                EditorUtility.SetDirty(_settings.currentGraph);
                EditorUtility.SetDirty(_settings.currentGraph.behavior);
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
                        type == typeof(RootNode) ||
                        type == typeof(Transition) ||
                        (type == typeof(InputNode) && !type.IsSubclassOf(typeof(InputNode))) ||
                        (type == typeof(LeafNode) && !type.IsSubclassOf(typeof(LeafNode))))
                        continue;

                    if (type.GetCustomAttribute(typeof(NodeAttribute)) is NodeAttribute nodeAttribute)
                    {
                        if (ContainsEntity(type)) continue;

                        NodeCreationEntity entity = new NodeCreationEntity(nodeAttribute.GetMenuName(),
                            nodeAttribute.GetDisplayName(), type, nodeAttribute.GetWidth());

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

        private void CheckRootNode()
        {
            // ReSharper disable once Unity.NoNullPropagation
            if (_settings?.currentGraph == null)
                return;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (BaseNodeSetting node in _settings.currentGraph.windows)
            {
                if (node.baseNode is RootNode)
                    return;
            }

            _settings.AddNodeOnGraph(_settings.fillerNode, new RootNode(), 50, 20, "Root", Vector2.zero);
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
                GUILayout.Label("Outputs");

                EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(maxWidth));

                foreach (FieldInfo output in outputs)
                {
                    OutputType attribute = Attribute.GetCustomAttribute(output, typeof(OutputType)) as OutputType;

                    if (attribute == null)
                        continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(attribute.name,
                        GUILayout.MaxWidth(style.CalcSize(new GUIContent(attribute.name)).x));

                    object obj = output.GetValue(selectedNode.baseNode);

                    GUILayout.FlexibleSpace();

                    object newValue = EditorMethods.InputField(attribute.type, obj);

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
            GUILayout.Label("Scale", GUILayout.Width(style.CalcSize(new GUIContent("Scale")).x));
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
                    n.baseNode ??= _settings.currentGraph.behavior.GetNodeByID(n.id);

                    n.DrawCurve(
                        _settings.currentGraph.behavior.GetNodeByID(
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
                _settings.currentGraph.behavior.GetNodeByID(id)
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

        public void MakeInformationTransition(BaseNodeSetting setting, int infoID, Type type, Vector2 pos,
            bool isTarget)
        {
            Transition t = (Transition) drawTrans?.baseNode;
            if (t is {transferInformation: false})
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
                drawTrans.type = type;
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

                if (!CheckTransitionAllow(type, isTarget, setting))
                {
                    if (_settings.currentGraph.windows.Contains(drawTrans))
                        _settings.currentGraph.DeleteNode(drawTrans.id);
                    drawTrans = null;
                    return;
                }

                Transition transition = (Transition) drawTrans.baseNode;

                transition.Set(node, infoID, isTarget);

                drawTrans.SetDraws(isTarget, setting, pos);

                if (transition.targetNodeID == -1 || transition.fromNodeID == -1) return;

                drawTrans.AddTransitionID(transition.fromNodeID);

                foreach (BaseNode toSet in _settings.currentGraph.behavior.nodes.Where(n =>
                    n.id == transition.fromNodeID))
                    toSet.AddTransition(transition);

                drawTrans = null;
            }
        }

        public void MakeActionTransition(BaseNodeSetting setting, bool isTarget, Vector2 pos, int index = -1)
        {
            Transition t = (Transition) drawTrans?.baseNode;
            if (t is {transferInformation: true})
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

                transition.Set(setting.baseNode, isTarget);

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

                if (!CheckTransitionAllow(null, isTarget, setting))
                {
                    if (_settings.currentGraph.windows.Contains(drawTrans))
                        _settings.currentGraph.DeleteNode(drawTrans.id);
                    drawTrans = null;
                    return;
                }

                Transition transition = (Transition) drawTrans.baseNode;

                transition.Set(setting.baseNode, isTarget);

                drawTrans.SetDraws(isTarget, setting, pos);

                if (transition.targetNodeID == -1 || transition.fromNodeID == -1) return;

                drawTrans.AddTransitionID(transition.fromNodeID);

                foreach (BaseNode toSet in _settings.currentGraph.behavior.nodes.Where(n =>
                    n.id == transition.fromNodeID))
                    toSet.AddTransition(transition);

                drawTrans = null;
            }
        }

        private bool CheckTransitionAllow(Type type, bool isTarget, BaseNodeSetting setting)
        {
            if ((isTarget && drawTrans.enterDraw != null) || (!isTarget && drawTrans.exitDraw != null))
                return false;
            if (setting.id == drawTrans.enterID || setting.id == drawTrans.exitID)
                return false;
            if (drawTrans.enterID != -1 || drawTrans.exitID != -1)
            {
                if (drawTrans.type != null && type != null)
                {
                    if (drawTrans.type != type)
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Context Menus

        private void AddNewNode(Event e)
        {
            if (_settings == null)
                return;
            GenericMenu menu = new GenericMenu();
            menu.AddDisabledItem(new GUIContent("Add Node"));

            // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
            if (_settings.currentGraph != null)
            {
                foreach (NodeCreationEntity nodeCreationEntity in creationList)
                {
                    menu.AddItem(new GUIContent(nodeCreationEntity.GetMenuName()), false, ContextCallback,
                        nodeCreationEntity);
                }

                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("Effect"));
                menu.AddItem(new GUIContent("Reset Panning"), false, ContextCallback, false);
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
                menu.AddDisabledItem(new GUIContent("Modify"));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, ContextCallback, true);
                menu.AddSeparator("");
            }

            menu.ShowAsContext();
            e.Use();
        }

        private void ContextCallback(object o)
        {
            try
            {
                NodeCreationEntity nodeEntity = (NodeCreationEntity) o;

                BaseNode node = Activator.CreateInstance(nodeEntity.GetNodeType()) as BaseNode;
                DrawNode drawNode;

                if (nodeEntity.GetNodeType().IsSubclassOf(typeof(InputNode)))
                    drawNode = _settings.inputNode;
                else if (nodeEntity.GetNodeType().IsSubclassOf(typeof(LeafNode)))
                    drawNode = _settings.leafNode;
                else
                    drawNode = _settings.fillerNode;


                _settings.AddNodeOnGraph(drawNode, node, nodeEntity.GetWidth(), 25, nodeEntity.GetDisplayName(),
                    mousePosition);
            }
            catch
            {
                try
                {
                    bool statement = (bool) o;

                    if (statement)
                        _settings.currentGraph.DeleteNode(selectedNode.id);
                    else
                        ResetScroll();
                }
                catch(Exception e)
                {
                    Debug.Log(e.Message);
                    Debug.Log("ContextCallback Input Error");
                }
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
        private readonly float width;

        public NodeCreationEntity(string menuName, string displayName, Type nodeType, float width)
        {
            this.menuName = menuName;
            this.displayName = displayName;
            this.nodeType = nodeType;
            this.width = width;
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

        public float GetWidth()
        {
            return width;
        }
    }
}