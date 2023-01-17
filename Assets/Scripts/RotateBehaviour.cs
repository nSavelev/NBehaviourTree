using NBehaviourTree.Runtime;
using UnityEngine;

namespace DefaultNamespace
{
    public class RotateBehaviour : BaseLeafNode<TestSMData>
    {
        [SerializeField]
        private float _speed = 10f;

        protected override BehaviourStatus OnRun()
        {
            var toTarget = Data.TargetPosition - Data.Position;
            if (Vector3.Angle(Data.Forward, toTarget) > 0.2f)
            {
                Data.Self.rotation = Quaternion.RotateTowards(Quaternion.LookRotation(Data.Forward), Quaternion.LookRotation(toTarget), _speed * Data.DeltaTime);
            }
            else
            {
                return BehaviourStatus.Failure;
            }

            return Vector3.Angle(Data.Forward, Data.TargetPosition - Data.Position) > 0.2f
                ? BehaviourStatus.Running
                : BehaviourStatus.Success;
        }
    }
}