using UnityEngine;

namespace NBehaviourTree.Runtime
{
    public class DebugNode : BaseLeafNode<IStateMachineData>
    {
        [SerializeParam]
        public string Message;
        protected override BehaviourStatus OnRun()
        {
            Debug.Log(Message);
            return BehaviourStatus.Success;
        }
    }
}