using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace KnowledgeProduction
{
    public abstract class KnowInstance
    {
        //Properties
        public int ID { get; private set; }
        public abstract object Content { get;}

        //Constructor
        public KnowInstance(int id)
        {
            this.ID = id;
        }

        //Debug
        public override string ToString()
        {
            //return string.Format("{0}", this.ID);
            return this.ID.ToString();
        }
        

    }
}
