using System.Linq;

namespace NBehaviourTree.Runtime.Compositors
{
    public class Parallel : BaseCompositeNode
    {
        protected override void OnInit(IStateMachineData data)
        {
        }

        protected override BehaviourStatus OnRun()
        {
            var results = Childs.Select(e => StateMachine.RunNode(e)).ToList();
            if (results.All(e => e == BehaviourStatus.Success))
                return BehaviourStatus.Success;
            if (results.Any(e => e == BehaviourStatus.Running))
                return BehaviourStatus.Running;
            return BehaviourStatus.Failure;
        }
    }
}