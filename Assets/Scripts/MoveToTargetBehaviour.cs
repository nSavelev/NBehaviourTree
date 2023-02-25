using NBehaviourTree.Runtime;
using UnityEngine;

namespace DefaultNamespace
{
    public class MoveToTargetBehaviour : BaseLeafNode<TestSMData>
    {
        [SerializeField]
        protected float _moveSpeed = 5f;

        protected override BehaviourStatus OnRun()
        {
            if (Vector3.Distance(Data.Position, Data.TargetPosition) < 0.1f)
                return BehaviourStatus.Failure;
            var toTarget = Data.TargetPosition - Data.Position;
            Data.Self.transform.position += toTarget.normalized * (_moveSpeed * Data.DeltaTime);
            if (Vector3.Distance(Data.Position, Data.TargetPosition) < 0.1f)
                return BehaviourStatus.Success;
            return BehaviourStatus.Running;
        }
    }
}