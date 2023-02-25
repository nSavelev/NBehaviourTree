using System;
using System.Collections.Generic;
using System.Linq;

namespace NBehaviourTree.Runtime
{
    public class StateMachine
    {
        public string RootNode => _rootNode;
        public IReadOnlyDictionary<string, BaseNode> Nodes => _nodes;

        public IReadOnlyDictionary<string, EditorData> EditorDatas =>
            _editorData.ToDictionary(e => e.Id, e => e);
        internal Dictionary<string, BaseNode> _nodes = new Dictionary<string, BaseNode>();
        internal List<EditorData> _editorData = new List<EditorData>();
        private string _rootNode;

        public void Init(IStateMachineData data)
        {
            foreach (var keyValuePair in _nodes)
            {
                keyValuePair.Value.Init(this, data);
            }
        }
        
        public BehaviourStatus RunNode(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BehaviourStatus.Success;
            if (_nodes.TryGetValue(id, out var node))
            {
                return node.Run();
            }
            return BehaviourStatus.Failure;
        }

        public void AddNode(BaseNode node)
        {
            _nodes.Add(node.ID, node);
        }

        public void AddNode(BaseNode node, EditorData editorData)
        {
            _nodes.Add(node.ID, node);
            _editorData.Add(editorData);
        }

        public void SetRoot(string id)
        {
            _rootNode = id;
        }

        public void RemoveNode(BaseNode node)
        {
            _nodes.Remove(node.ID);
            _editorData.RemoveAll(e => e.Id == node.ID);
            if (node.ID == _rootNode)
                _rootNode = string.Empty;
            if (!string.IsNullOrEmpty(node.Parent))
            {
                if (_nodes.TryGetValue(node.Parent, out var parentNode))
                {
                    parentNode.RemoveChild(node.ID);
                }
            }

            foreach (var leftNode in _nodes.Values)
            {
                if (leftNode.HasInChilds(node.ID))
                {
                    leftNode.RemoveChild(node.ID);
                }
            }
        }

        public void SetAsChildren(BaseNode parent, BaseNode children, int index = 0)
        { 
            if (!string.IsNullOrEmpty(children.Parent))
            {
                if (_nodes.TryGetValue(children.Parent, out var prnt))
                {
                    prnt.RemoveChild(children.ID);
                }
                else
                {
                    children.Parent = String.Empty;
                }
            }
            switch (parent)
            {
                case BaseDecoratorNode leaf:
                    leaf.Child = children.ID;
                    children.SetParent(leaf.ID);
                    break;
                case BaseCompositeNode composite:
                    children.SetParent(composite.ID);
                    composite.Childs[index] = children.ID;
                    break;
            }
        }
    }
}