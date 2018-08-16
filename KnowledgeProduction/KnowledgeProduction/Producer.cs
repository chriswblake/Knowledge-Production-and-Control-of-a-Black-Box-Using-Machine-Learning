using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace KnowledgeProduction
{
    public class Producer
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

        //Properties
        public Func<int> GenerateIdDelegate { get; set; } 

        //Cache - Properties
        private int _lastID = 0;

        //Constructors
        public Producer()
        {
            //Default ID generator.
            GenerateIdDelegate = delegate ()
            {
                _lastID++;
                return _lastID;
            };
        }

        //Methods   
        private int GenerateId()
        {
            return GenerateIdDelegate();
        }
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
            if (_prevInstance != null)
            { 
                //Generate theoretical pair hashcode
                int key = KnowInstanceSymbol.GetTheoreticalHashCode(_prevInstance, theInstance);

                //If id does not exist, created it and return it.
                if (!knowInstances.ContainsKey(key))
                    SaveInstance(new KnowInstanceSymbol(GenerateId(), _prevInstance, theInstance));
            }

            //Shift current instance to previous instances
            _prevInstance = theInstance;
        }

        //Cache - Methods
        private KnowInstance _prevInstance = null;

    }
}
