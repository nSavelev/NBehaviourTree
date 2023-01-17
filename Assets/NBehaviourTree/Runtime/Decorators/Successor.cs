namespace NBehaviourTree.Runtime.Decorators
{
    public class Successor : BaseDecoratorNode
    {
        protected override void OnInit(IStateMachineData data)
        {
        }

        protected override BehaviourStatus OnRun()
        {
            StateMachine.RunNode(Child);
            return BehaviourStatus.Success;
        }
    }
}