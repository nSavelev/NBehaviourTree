using DefaultNamespace;
using NBehaviourTree.Runtime;
using NBehaviourTree.Runtime.Comparator;
using UnityEngine;

public class LookCheck : DecoratorNode<TestSMData>
{
    [SerializeField]
    protected float _angle = 5f;

    [SerializeField]
    protected CompareFunc _compareFunc;

    protected override BehaviourStatus OnRun()
    {
        var toVector = Data.TargetPosition - Data.Position;
        var angle = Vector3.Angle(toVector, Data.Forward); 
        return Comparator.Compare(_compareFunc, angle, _angle) ? RunChild() : BehaviourStatus.Failure;
    }
}