using System;
using System.Collections.Generic;
using System.Linq;
using NBehaviourTree.Runtime.Compositors;
using NBehaviourTree.Runtime.Decorators;
using UnityEngine;

namespace NBehaviourTree.Runtime.Serialization
{
    public static class NStateMachineSerializer
    {
        private static readonly Dictionary<string, Type> _registeredTypes = new Dictionary<string, Type>()
        {
            [typeof(Selector).FullName] = typeof(Selector),
            [typeof(Sequence).FullName] = typeof(Sequence),
            [typeof(Invertor).FullName] = typeof(Invertor),
            [typeof(RepeatUntilFail).FullName] = typeof(RepeatUntilFail),
            [typeof(Successor).FullName] = typeof(Successor),
            [typeof(Parallel).FullName] = typeof(Parallel),
        };
        
        [Serializable]
        private class SerializationWrap
        {
            public string Type;
            public string Data;
        }

        [Serializable]
        private class StateMachineWrap
        {
            public string RootNode;
            public SerializationWrap[] Nodes;
            public EditorData[] EditorDatas;
        }
        
        public static string Serialize(StateMachine data)
        {
            var smData = new StateMachineWrap()
            {
                Nodes = data._nodes.Values.Select(e => new SerializationWrap()
                {
                    Type = e.GetType().FullName,
                    Data = JsonUtility.ToJson(e)
                }).ToArray(),
                RootNode = data.RootNode,
                EditorDatas = data._editorData.ToArray(),
            };
            // return JsonConvert.SerializeObject(smData);
            return JsonUtility.ToJson(smData);
        }

        public static StateMachine Deserialize(string data, params System.Type[] types)
        {
            var sm = new StateMachine();
            var wrap = JsonUtility.FromJson<StateMachineWrap>(data);
            // var wrap = JsonConvert.DeserializeObject<StateMachineWrap>(data);
            var allTypes = _registeredTypes.ToDictionary(e => e.Key, e => e.Value);
            foreach (var type in types)
            {
                allTypes[type.FullName] = type;
            }
            foreach (var node in wrap.Nodes)
            {
                if (allTypes.TryGetValue(node.Type, out var type))
                {
                    sm.AddNode((BaseNode)JsonUtility.FromJson(node.Data, type));
                }
            }
            sm._editorData = wrap.EditorDatas.ToList();
            sm.SetRoot(wrap.RootNode);
            return sm;
        }
    }
}