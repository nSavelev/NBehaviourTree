using System;
using System.Collections.Generic;
using System.Linq;
using NBehaviourTree.Runtime;
using UnityEditor;
using UnityEngine;

namespace NBehaviourTree.Editor.StateMachineEditor
{
    public class NBehaviorEditorWindow : EditorWindow
    {
        [SerializeField]
        internal Texture _rootNodeTexture;

        [SerializeField]
        internal Texture _nodePointTexture;

        private StateMachine _stateMachine;
        private StateMachineAsset _asset;
        private Vector2 _offset;
        internal RootNode _root;
        internal ConnectionDraw ConnectionDraw { get; private set; }
    

        internal Dictionary<string, Node> _nodes = new Dictionary<string, Node>();
        private Dictionary<string, Type> _nodeTypes = new Dictionary<string, Type>();
        private ObjectDataDrawer _selectedNodeDrawer;
        [SerializeField]
        private Rect _selectedRect = new Rect(10,50, 300, 100);

        [MenuItem("Assets/NBehaviourTree/Create")]
        public static void CreateNewTree()
        {
            var wnd = CreateWindow<NBehaviorEditorWindow>();
            wnd.minSize = new Vector2(800, 600);
            wnd.Init(new StateMachine());
        }

        public static void Create(StateMachineAsset stateMachineAsset)
        {
            var wnd = CreateWindow<NBehaviorEditorWindow>();
            wnd.minSize = new Vector2(800, 600);
            wnd._stateMachine = stateMachineAsset.Get(GetNodeTypes());
            wnd._asset = stateMachineAsset;
            wnd.Init(stateMachineAsset);
            foreach (var node in wnd._stateMachine.Nodes)
            {
                var editorData = wnd._stateMachine.EditorDatas[node.Key];
                wnd._nodes.Add(node.Value.ID, new Node(editorData.Position, node.Value, editorData, wnd));
            }
        }

        private static Type[] GetNodeTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                    .Where(type => typeof(BaseNode).IsAssignableFrom(type) && !type.IsAbstract))
                .ToArray();
        }

        private void Init(StateMachineAsset asset)
        {
            var types = GetNodeTypes();
            _stateMachine = asset.Get(types);
            _asset = asset;
            _nodeTypes.Clear();
            _root = new RootNode(this);
            FillMenu(types);
        }

        private void FillMenu(Type[] types)
        {
            foreach (var type in types)
            {
                if (typeof(BaseCompositeNode).IsAssignableFrom(type))
                {
                    _nodeTypes.Add($"Composites/{type.Name}", type);
                }

                if (typeof(BaseDecoratorNode).IsAssignableFrom(type))
                {
                    _nodeTypes.Add($"Decorators/{type.Name}", type);
                }

                if (typeof(AbstractLeaf).IsAssignableFrom(type))
                {
                    _nodeTypes.Add($"Leafs/{type.Name}", type);
                }
            }
        }
        
        private void Init(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            var types = GetNodeTypes();
            _nodeTypes.Clear();
            _root = new RootNode(this);
            FillMenu(types);
        }

        void OnGUI()
        {
            DrawWorkingArea();
            ProcessNodeEvents(Event.current);
            ProcessEvent(Event.current);
            DrawCurrentConnection(Event.current);
            DrawNodesConnections();
            DrawMenu();
            DrawNodeEdit();
            if (GUI.changed)
                Repaint();
        }

        private void DrawNodeEdit()
        {
            if (_selectedNodeDrawer == null)
                return;
            _selectedNodeDrawer.Draw(ref _selectedRect);
        }

        private void DrawNodesConnections()
        {
            foreach (var node in _stateMachine.Nodes.Values)
            {
                switch (node)
                {
                    case BaseCompositeNode compositeNode:
                        for (int i = 0; i < compositeNode.MaxChilds; i++)
                        {
                            if (!string.IsNullOrEmpty(compositeNode.Childs[i]))
                            {
                                ConnectionDraw.DrawConnection(_nodes[compositeNode.ID].GetFromPoint(i), _nodes[compositeNode.Childs[i]].GetInPoint());
                            }
                        }
                        break;
                    case BaseDecoratorNode decoratorNode:
                        if (!string.IsNullOrEmpty(decoratorNode.Child))
                        {
                            ConnectionDraw.DrawConnection(_nodes[decoratorNode.ID].GetFromPoint(0),
                                _nodes[decoratorNode.Child].GetInPoint());
                        }
                        break;
                }
            }
        }

        private void DrawCurrentConnection(Event current)
        {
            if (ConnectionDraw != null)
            {
                ConnectionDraw.Draw(Event.current);
            }
        }

        private void DrawWorkingArea()
        {
            var areaRect = new Rect(0, 36, this.position.width, this.position.height - 36);
            EditorGUI.DrawRect(areaRect, Color.gray);
            DrawNodes(areaRect);
            DrawConnections();
        }

        private void DrawConnections()
        {
            if (!string.IsNullOrEmpty(_stateMachine.RootNode))
            {
                ConnectionDraw.DrawConnection(_root.RootConnectionRect.center, _nodes[_stateMachine.RootNode].GetInPoint());
            }
        }

        private void DrawNodes(Rect areaRect)
        {
            DrawRootNode(areaRect);
            foreach (var node in _nodes.Values)
            {
                node.Draw(_offset);
            }
        }

        private void DrawRootNode(Rect areaRect)
        {
            _root.Draw(_offset + areaRect.center - Vector2.up * 200);
        }

        private void DrawMenu()
        {
            EditorGUILayout.BeginHorizontal("Box", GUILayout.Height(20));
            if (GUILayout.Button("Save"))
            {
                _asset.Set(_stateMachine);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ProcessNodeEvents(Event evnt)
        {
            if (_root.ProcessEvent(evnt))
            {
                GUI.changed = true;
            }
            foreach (var node in _nodes.Values)
            {
                if (node.ProcessEvent(evnt))
                {
                    GUI.changed = true;
                }
            }
        }

        private void ProcessEvent(Event evnt)
        {
            switch (evnt.type)
            {
                case EventType.MouseDown:
                    if (evnt.button == 1)
                    {
                        ProcessContextMenu(evnt.mousePosition);
                    }
                    if (evnt.button == 0)
                    {
                        if (_selectedNodeDrawer == null || !_selectedRect.Contains(evnt.mousePosition))
                        {
                            _selectedNodeDrawer = null;
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    if (evnt.button == 2)
                    {
                        _offset += evnt.delta;
                        GUI.changed = true;
                    }
                    break;
                case EventType.MouseUp:
                    if (evnt.button == 0)
                    {
                        if (ConnectionDraw != null)
                        {
                            EndConnection();
                            ConnectionDraw = null;
                        }
                    }
                    break;
            }
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            var menu = new GenericMenu();
            foreach (var nodeType in _nodeTypes)
            {
                menu.AddItem(new GUIContent(nodeType.Key), false, () => { CreateNode(mousePosition, nodeType.Value); });
            }
            menu.ShowAsContext();
        }

        private void CreateNode(Vector2 mousePosition, Type nodeType)
        {
            var node = (BaseNode)Activator.CreateInstance(nodeType);
            node.ID = Guid.NewGuid().ToString();
            var editorData = new EditorData()
            {
                Id = node.ID,
                Position = mousePosition
            };
            _stateMachine.AddNode(node, editorData);
            _nodes.Add(node.ID, new Node(mousePosition, node, editorData, this));
        }

        public void RemoveNode(BaseNode node)
        {
            _stateMachine.RemoveNode(node);
            _nodes.Remove(node.ID);
        }

        public void BeginConnection(string from, string to, int index = 0)
        {
            if (string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to))
                return;
            ConnectionDraw = new ConnectionDraw(this)
            {
                From = from,
                To = to,
                FromIndex = index
            };
        }

        public const string ROOD_ID = "root_id";
        public void EndConnection(int index = 0)
        {
            var from = ConnectionDraw.From;
            var to = ConnectionDraw.To;
            if (string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to))
            {
                ConnectionDraw = null;
                return;
            }

            if (from == ROOD_ID)
            {
                _stateMachine.SetRoot(ConnectionDraw.To);
                ConnectionDraw = null;
                return;
            }
            if (!string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to))
            {
                _stateMachine.Nodes[from].RemoveChildAt(index);
                ConnectionDraw = null;
                return;
            }
            if (!string.IsNullOrEmpty(to) && string.IsNullOrEmpty(from))
            {
                if (_stateMachine.RootNode == to)
                {
                    _stateMachine.SetRoot(String.Empty);
                }
                _stateMachine.Nodes[to].SetParent(String.Empty);
                ConnectionDraw = null;
                return;
            }
            _stateMachine.SetAsChildren(_stateMachine.Nodes[from], _stateMachine.Nodes[to], ConnectionDraw.FromIndex);
            ConnectionDraw = null;
        }

        public void NodeClicked(Node node)
        {
            _selectedNodeDrawer = new ObjectDataDrawer(node.StateNode);
        }
    }
}