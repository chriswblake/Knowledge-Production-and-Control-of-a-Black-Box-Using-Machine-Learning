using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace KnowledgeProduction
{
    public class Identifier
    {
        //Fields
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<int,KnowInstance> knowInstances = new Dictionary<int, KnowInstance>();
        public Dictionary<int,KnowInstance>.ValueCollection KnowInstances
        {
            get
            {
                return this.knowInstances.Values;
            }
        }

        private KnowInstance prevInstance = null;

        //Methods        
        public void Learn(KnowInstance theInstance)
        {
            SaveInstance(theInstance);
            SaveSequence(theInstance);
        }
        
        public void SaveInstance(KnowInstance theInstance)
        {
            //If not known, add it to the the list of known items.
            if (!knowInstances.ContainsKey(theInstance.GetHashCode()))
                knowInstances.Add(theInstance.GetHashCode(), theInstance);
        }
        public void SaveSequence(KnowInstance theInstance)
        {
            if (prevInstance != null)
            { 
                //Generate theoretical pair hashcode
                int key = KnowInstanceSymbol.GetTheoreticalHashCode(prevInstance, theInstance);

                //If id does not exist, created it and return it.
                if (!knowInstances.ContainsKey(key))
                    SaveInstance(new KnowInstanceSymbol(prevInstance, theInstance));
            }

            //Shift current instance to previous instances
            prevInstance = theInstance;
        }
    }
}
