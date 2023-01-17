using System;
using System.Collections.Generic;
using System.Linq;

namespace NBehaviourTree.Runtime
{
    public abstract class BaseCompositeNode : BaseNode
    {
        public virtual int MaxChilds => 5;
        public string[] Childs;
        public override IEnumerable<string> OutNodes => Childs;

        public BaseCompositeNode()
        {
            Childs = new string[MaxChilds];
        }

        public override bool HasInChilds(string nodeId)
        {
            return Childs.Any(e => e == nodeId);
        }

        public override void RemoveChild(string nodeID)
        {
            for (int i = 0; i < Childs.Length; i++)
            {
                if (Childs[i] == nodeID)
                {
                    Childs[i] = string.Empty;
                }
            }
        }

        public override void RemoveChildAt(int index)
        {
            if (Childs.Length > index && index >= 0)
                Childs[index] = String.Empty;
        }
    }
}