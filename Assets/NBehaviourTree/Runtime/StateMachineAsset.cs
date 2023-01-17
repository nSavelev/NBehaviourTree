using System;
using NBehaviourTree.Runtime.Serialization;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NBehaviourTree.Runtime
{
    [CreateAssetMenu(fileName = "New NBehaviourTree", menuName = "NBehaviourTree/NewAsset")]
    public class StateMachineAsset : ScriptableObject
    {
        [SerializeField]
        private string _stateMachineData;

        public StateMachine Get(params Type[] types)
        {
            if (string.IsNullOrEmpty(_stateMachineData))
                return new StateMachine();
            return NStateMachineSerializer.Deserialize(_stateMachineData, types);
        }

        #if UNITY_EDITOR
        public void Set(StateMachine stateMachine)
        {
            _stateMachineData = NStateMachineSerializer.Serialize(stateMachine);
            EditorUtility.SetDirty(this);
        }
        #endif
    }
}