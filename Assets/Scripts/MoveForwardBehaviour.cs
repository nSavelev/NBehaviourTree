using DefaultNamespace;
using NBehaviourTree.Runtime;
using UnityEngine;

public class MoveForwardBehaviour : BaseLeafNode<TestSMData>
{
    [SerializeField]
    protected float _speed;
    
    protected override BehaviourStatus OnRun()
    {
        Data.Self.position += Data.Forward * (_speed * Data.DeltaTime);
        return BehaviourStatus.Running;
    }
}