namespace NBehaviourTree.Runtime.Compositors
{
    public class Selector : BaseCompositeNode
    {
        private int _currentIndex = -1;
        
        protected override void OnInit(IStateMachineData data)
        {
        }

        protected override BehaviourStatus OnRun()
        {
            var status = BehaviourStatus.None;
            if (_currentIndex < 0)
            {
                for (int i = 0; i < Childs.Length; i++)
                {
                    var childStatus = StateMachine.RunNode(Childs[i]);
                    if (childStatus != BehaviourStatus.Failure)
                    {
                        _currentIndex = i;
                        return BehaviourStatus.Running;
                    }
                }
                return BehaviourStatus.Failure;
            }
            else
            {
                var childResult = StateMachine.RunNode(Childs[_currentIndex]);
                if (childResult == BehaviourStatus.Success || childResult == BehaviourStatus.Failure)
                {
                    Reset();
                    return childResult;
                }
            }
            return status;
        }

        private void Reset()
        {
            _currentIndex = -1;
        }
    }
}