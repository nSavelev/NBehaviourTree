using System;

namespace NBehaviourTree.Runtime.Compositors
{
    public class Sequence : BaseCompositeNode
    {
        private int _lastIndex;
        protected override void OnInit(IStateMachineData data)
        {
        }

        protected override BehaviourStatus OnRun()
        {
            var status = BehaviourStatus.Running;
            if (_lastIndex < Childs.Length)
            {
                var childStatus = StateMachine.RunNode(Childs[_lastIndex]);
                switch (childStatus)
                {
                    case BehaviourStatus.None:
                        throw new Exception($"Invalid status for child node {status}");
                        break;
                    case BehaviourStatus.Success:
                        _lastIndex++;
                        return OnRun();
                        break;
                    case BehaviourStatus.Failure:
                        Reset();
                        break;
                    case BehaviourStatus.Running:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                Reset();
                status = BehaviourStatus.Success;
            }
            return status;
        }

        private void Reset()
        {
            _lastIndex = 0;
        }
    }
}