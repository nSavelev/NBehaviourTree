using System;
using NBehaviourTree.Runtime;
using UnityEngine;

namespace DefaultNamespace
{
    public class TestSMData : IStateMachineData
    {
        public Vector3 Position => _self.position;
        public Vector3 Forward => _self.forward;
        public Vector3 TargetPosition => _target.position;
        public Transform Self => _self;
        public float DeltaTime { get; set; }

        private readonly Transform _self;
        private readonly Transform _target;

        public TestSMData(Transform self, Transform target)
        {
            _target = target;
            _self = self;
        }
    }

    public class TestStateMachine : StateMachineRunner<TestSMData>
    {
        [SerializeField]
        private Transform _target;
        
        private TestSMData _data;
        protected override Type[] StateTypes => new[] { typeof(MoveBehaviour), typeof(RotateBehaviour) };
        protected override TestSMData Data => _data;
        protected override void OnInit()
        {
            _data = new TestSMData(transform, _target);
        }

        protected override void OnUpdate()
        {
            _data.DeltaTime = Time.deltaTime;
        }
    }
}