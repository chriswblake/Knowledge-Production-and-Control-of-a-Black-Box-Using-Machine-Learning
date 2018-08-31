using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace KnowledgeProduction
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Producer
    {
        //Fields
        private object addremoveLock = new object();
        public Dictionary<int,KnowInstance> KnowInstances = new Dictionary<int, KnowInstance>(); //Stored by hashcode, which is based on IDs.
        
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

        //Methods - Usage
        public KnowInstance Get(KnowInstance k1, KnowInstance k2)
        {
            //If either is null
            if (k1 == null || k2 == null)
                return null;

            var kis = new KnowInstanceSymbol(-1, new List<KnowInstance> { k1, k2 });
            return Get(kis.GetHashCode());
        }
        public KnowInstance Get(int hashcode)
        {
            //Check if code is valid
            if (!KnowInstances.ContainsKey(hashcode)) return null;

            return KnowInstances[hashcode];
        }

        //Methods - Learning
        public void Learn(List<KnowInstance> knowInstances)
        {
            var kis = new KnowInstanceSymbol(GenerateId(), knowInstances);
            Learn(kis);
        }
        public void Learn(KnowInstance theInstance)
        {
            if (theInstance == null) return;

            SaveInstance(theInstance);
            SaveSequence(theInstance);
        }
        protected void SaveInstance(KnowInstance theInstance)
        {
            lock(addremoveLock)
            { 
            //If not known, add it to the the list of known items.
            if (!KnowInstances.ContainsKey(theInstance.GetHashCode()))
                KnowInstances.Add(theInstance.GetHashCode(), theInstance);
            }
        }
        protected void SaveSequence(KnowInstance theInstance)
        {
            lock (addremoveLock)
            {
                if (_prevInstance != null)
                {
                    //Generate theoretical pair hashcode
                    int key = KnowInstanceSymbol.GetTheoreticalHashCode(_prevInstance, theInstance);

                    //If id does not exist, created it and return it.
                    if (!KnowInstances.ContainsKey(key))
                        SaveInstance(new KnowInstanceSymbol(GenerateId(), _prevInstance, theInstance));
                }
            
                //Shift current instance to previous instances
                _prevInstance = theInstance;
            }
        }
        private int GenerateId()
        {
            return GenerateIdDelegate();
        }

        //Methods - Updating
        public void Add(int id, object value)
        {
            lock (addremoveLock)
            {
                KnowInstanceValue kiv = new KnowInstanceValue(id, value);
                SaveInstance(kiv);
            }
        }
        public void Remove(int id)
        {
            lock (addremoveLock)
            {
                //Remove all instances that contain this instance
                var symbols = KnowInstances.Where(d => d.Value.GetType() == typeof(KnowInstanceSymbol)).ToList(); //Find only symbols
                symbols = symbols.Where(k => ((KnowInstanceSymbol)k.Value).Contains(id)).ToList(); //Only symobols containing the id
                foreach (var s in symbols)
                    KnowInstances.Remove(s.Key);

                //Remove the item
                if (KnowInstances.ContainsKey(id))
                    KnowInstances.Remove(id);

                //Check if stored in cash
                if (_prevInstance != null && _prevInstance.ID == id)
                    _prevInstance = null;
            }
        }

        //Cache - Methods
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _lastID = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private KnowInstance _prevInstance = null;

        //Debug
        public override string ToString()
        {
            return DebuggerDisplay;
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplay
        {
            get
            {
                return string.Format("KnowInst={0}", this.KnowInstances.Count);
            }
        }

    }
}
