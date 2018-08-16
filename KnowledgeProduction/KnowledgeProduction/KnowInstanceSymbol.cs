using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace KnowledgeProduction
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class KnowInstanceSymbol : KnowInstance
    {
        //Properties
        private List<KnowInstance> _content = new List<KnowInstance>();
        public override object Content { get
            {
                return _content;
            } }

        //Constructor
        public KnowInstanceSymbol(int id, List<KnowInstance> knowInstances) : base(id)
        {
            foreach (KnowInstance ki in knowInstances)
                _content.Add(ki);
        }
        public KnowInstanceSymbol(int id, KnowInstance instanceA, KnowInstance instanceB) : base(id)
        {
            _content.Add(instanceA);
            _content.Add(instanceB);
        }

        //Methods
        public override int GetHashCode()
        {
            return GetIDs(_content).GetHashCode();
        }
        public static int GetTheoreticalHashCode(KnowInstance instanceA, KnowInstance instanceB)
        {
            List<KnowInstance> instances = new List<KnowInstance> { instanceA, instanceB };
            return GetIDs(instances).GetHashCode();
        }
        private static string GetIDs(List<KnowInstance> instances)
        {
            return string.Join(",", instances.Select(p => p.ID));
        }

        //Debug
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return string.Format("{0}:({1})", base.ToString(), GetIDs(_content));
            }
        }
        public override string ToString()
        {
            return string.Format("{0}:({1})", base.ToString(), GetIDs(_content));
        }

    }
}
