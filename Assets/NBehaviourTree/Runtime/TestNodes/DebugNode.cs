using UnityEngine;

namespace NBehaviourTree.Runtime
{
    public class DebugNode : BaseLeafNode<IStateMachineData>
    {
        [SerializeParam]
        private string _message;
        protected override BehaviourStatus OnRun()
        {
            Debug.Log(_message);
            return BehaviourStatus.Success;
        }
    }
}