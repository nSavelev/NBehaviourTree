namespace NBehaviourTree.Runtime.Decorators
{
    public class Invertor : BaseDecoratorNode
    {
        protected override void OnInit(IStateMachineData data)
        {
        }

        protected override BehaviourStatus OnRun()
        {
            var childStatus = RunChild();
            if (childStatus == BehaviourStatus.Failure)
                return BehaviourStatus.Success;
            if (childStatus == BehaviourStatus.Success)
                return BehaviourStatus.Failure;
            return childStatus;
        }

        public override bool HasInChilds(string nodeId)
        {
            return Child == nodeId;
        }

        public override void RemoveChild(string nodeID)
        {
            if (Child == nodeID)
                Child = string.Empty;
        }
    }
}