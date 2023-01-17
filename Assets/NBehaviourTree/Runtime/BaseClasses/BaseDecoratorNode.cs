
using System;
using System.Collections.Generic;
using System.Linq;

namespace NBehaviourTree.Runtime
{
    public abstract class BaseDecoratorNode : BaseNode
    {
        public override IEnumerable<string> OutNodes => Enumerable.Repeat<string>(Child, 1);

        [SerializeParam]
        public string Child;

        public override void RemoveChild(string nodeID)
        {
            if (HasInChilds(nodeID))
            {
                Child = String.Empty;
            }
        }

        public override void RemoveChildAt(int index)
        {
            if (index == 0)
            {
                Child = String.Empty;
            }
        }
    }
}