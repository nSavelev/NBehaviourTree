using DefaultNamespace;
using NBehaviourTree.Runtime;
using NBehaviourTree.Runtime.Comparator;
using UnityEngine;

public class DistanceCheck : DecoratorNode<TestSMData>
{
    [SerializeField]
    private float _distance = 0.5f;

    [SerializeField]
    protected CompareFunc _compareFunc;
        
    protected override void OnInit(IStateMachineData data)
    {
    }

    protected override BehaviourStatus OnRun()
    {
        var distance = Vector3.Distance(Data.Position, Data.TargetPosition); 
        return  Comparator.Compare(_compareFunc, distance, _distance)
            ? RunChild()
            : BehaviourStatus.Failure;
    }
}