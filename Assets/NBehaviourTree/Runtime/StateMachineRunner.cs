using System;
using UnityEngine;

namespace NBehaviourTree.Runtime
{
    public abstract class StateMachineRunner<TData> : MonoBehaviour where TData : IStateMachineData
    {
        [SerializeField]
        private StateMachineAsset _stateMachineAsset;

        private StateMachine _stateMachine;

        protected abstract Type[] StateTypes { get; }
        protected abstract TData Data { get; }

        // Start is called before the first frame update
        void Start()
        {
            _stateMachine = _stateMachineAsset.Get(StateTypes);
            OnInit();
            _stateMachine.Init(Data);
        }

        protected abstract void OnInit();

        // Update is called once per frame
        void Update()
        {
            OnUpdate();
            _stateMachine.RunNode(_stateMachine.RootNode);
        }

        protected abstract void OnUpdate();
    }
}
