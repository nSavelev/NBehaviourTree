namespace NBehaviourTree.Runtime
{
    public abstract class DecoratorNode<TData> : BaseDecoratorNode where TData : IStateMachineData
    {
        protected TData Data => (TData)base.Data;
        
        protected override void OnInit(IStateMachineData data)
        {
        }
    }
}