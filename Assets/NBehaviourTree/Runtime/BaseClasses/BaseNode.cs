using System;
using System.Collections.Generic;

namespace NBehaviourTree.Runtime
{
    public interface INode
    {
        bool CanStart();
        BehaviourStatus Run();
    }

    public abstract class BaseNode
    { 
        [SerializeParam]
        public string ID;
        [SerializeParam]
        public string Name;
        [SerializeParam]
        public string Parent;

        public abstract IEnumerable<string> OutNodes { get; }

        public BehaviourStatus Status { get; private set; }
        protected IStateMachineData Data;
        protected StateMachine StateMachine;

        public void SetParent(string parent)
        {
            Parent = parent;
        }
        
        public void Init(StateMachine stateMachine, IStateMachineData data)
        {
            StateMachine = stateMachine;
            Data = data;
        }

        protected abstract void OnInit(IStateMachineData data);

        public BehaviourStatus Run()
        {
            Status = OnRun();
            return Status;
        }

        protected abstract BehaviourStatus OnRun();

        public virtual void RemoveChild(string nodeID)
        {
        }

        public virtual bool HasInChilds(string nodeId)
        {
            return false;
        }

        public virtual void RemoveChildAt(int index)
        {
        }
    }
}