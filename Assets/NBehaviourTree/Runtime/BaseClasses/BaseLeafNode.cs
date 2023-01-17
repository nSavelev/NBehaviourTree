using System;
using System.Collections.Generic;
using System.Linq;

namespace NBehaviourTree.Runtime
{
    public abstract class AbstractLeaf : BaseNode{}
    
    public abstract class BaseLeafNode<TData> : AbstractLeaf where TData : IStateMachineData
    {
        public override IEnumerable<string> OutNodes => Enumerable.Empty<string>();
        protected TData Data => (TData)base.Data;
        protected override void OnInit(IStateMachineData data)
        {
        }
    }
}