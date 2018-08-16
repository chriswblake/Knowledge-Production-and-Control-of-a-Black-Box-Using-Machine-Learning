using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace KnowledgeProduction
{   
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class KnowInstanceValue : KnowInstance
    {
        //Properties
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object _value = null;
        public override object Content
        {
            get
            {
                return _value;
            }
        }

        //Constructor
        public KnowInstanceValue(int id, object value) : base(id)
        {
            this._value = value;
        }

        //Methods
        public override int GetHashCode()
        {
            return this.ID.ToString().GetHashCode();
        }

        //Debug
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return string.Format("{0}:{1}", base.ToString(), this.Content.ToString());
            }
        }


    }
}
