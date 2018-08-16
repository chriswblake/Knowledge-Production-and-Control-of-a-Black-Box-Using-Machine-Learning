using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace KnowledgeProduction
{
    public abstract class KnowInstance
    {
        //Fields
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static int _lastID = 0;

        //Properties
        public int ID { get; set; }
        public abstract object Content { get;}
        public bool IsAutoID { get; private set; } = true;

        //Constructor
        public KnowInstance()
        {
            //Assign a unique ID to this new instance
            _lastID++;
            this.ID = _lastID;
        }

        //Debug
        public override string ToString()
        {
            //return string.Format("{0}", this.ID);
            return this.ID.ToString();
        }
        

    }
}
