using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace KnowledgeProduction
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Producer
    {
        //Fields
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<int,KnowInstance> knowInstances = new Dictionary<int, KnowInstance>(); //Stored by hashcode, which is based on content;
        public Dictionary<int,KnowInstance>.ValueCollection KnowInstances
        {
            get
            {
                return this.knowInstances.Values;
            }
        }

        //Properties
        public Func<int> GenerateIdDelegate { get; set; } 

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

        //Methods - Learning
        public void Learn(KnowInstance theInstance)
        {
            SaveInstance(theInstance);
            SaveSequence(theInstance);
        }
        private int GenerateId()
        {
            return GenerateIdDelegate();
        }
        protected void SaveInstance(KnowInstance theInstance)
        {
            //If not known, add it to the the list of known items.
            if (!knowInstances.ContainsKey(theInstance.GetHashCode()))
                knowInstances.Add(theInstance.GetHashCode(), theInstance);
        }
        protected void SaveSequence(KnowInstance theInstance)
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

        //Methods - Updating
        public void Add(int id, object value)
        {
            KnowInstanceValue kiv = new KnowInstanceValue(id, value);
            SaveInstance(kiv);
        }
        public void Remove(int id)
        {
            if (knowInstances.ContainsKey(id))
                knowInstances.Remove(id);
        }

        //Cache - Methods
        private int _lastID = 0;
        private KnowInstance _prevInstance = null;

        //Debug
        public override string ToString()
        {
            return DebuggerDisplay;
        }
        public string DebuggerDisplay
        {
            get
            {
                return string.Format("KnowInst={0}", this.knowInstances.Count);
            }
        }

    }
}
