using System;
using NBehaviourTree.Runtime;
using NBehaviourTree.Runtime.Serialization;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class BehaviourTreeSerializationTest : MonoBehaviour
    {
        public enum TestEnum
        {
            A,
            B
        }
        
        public class TestLeaf : BaseLeafNode<IStateMachineData>
        {
            public string[] ShitShit;
            public string Shit;
            public int WTFValue;
            public TestEnum Enum;
            
            protected override BehaviourStatus OnRun()
            {
                return BehaviourStatus.Success;
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != GetType())
                    return false;
                var node = (TestLeaf)obj;
                var result = true;
                result = result && Enum == node.Enum;
                result = result && Shit == node.Shit;
                result = result && WTFValue == node.WTFValue;
                result = result && ShitShit.Length == node.ShitShit.Length;
                return result;
            }
        }
        
        [Test]
        public void SimpleConversionTest()
        {
            var leaf = new TestLeaf()
            {
                ShitShit = new[] { "1", "2" },
                Shit = "shit",
                Enum = TestEnum.B,
                ID = Guid.NewGuid().ToString(),
                Name = "TestLeaf",
                WTFValue = 666
            };
            var sm = new StateMachine();
            sm.AddNode(leaf);
            var data = NStateMachineSerializer.Serialize(sm);
            Debug.Log(data);
            var smb = NStateMachineSerializer.Deserialize(data, typeof(TestLeaf));
            Assert.True(CompareStateMachines(sm, smb));
        }

        public bool CompareStateMachines(StateMachine a, StateMachine b)
        {
            if (a.Nodes.Count != b.Nodes.Count)
                return false;
            foreach (var node in b.Nodes)
            {
                if (!a.Nodes.TryGetValue(node.Key, out var nodeB))
                {
                    return false;
                }

                if (!node.Value.Equals(nodeB))
                    return false;
            }
            return true;
        }
    }
}
