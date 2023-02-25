namespace NBehaviourTree.Runtime.Decorators
{
    public class RepeatUntilFail : BaseDecoratorNode
    {
        protected override void OnInit(IStateMachineData data)
        {
        }

        protected override BehaviourStatus OnRun()
        {
            var childStatus = RunChild();
            return childStatus == BehaviourStatus.Failure ? BehaviourStatus.Success : BehaviourStatus.Running;
        }
    }
}