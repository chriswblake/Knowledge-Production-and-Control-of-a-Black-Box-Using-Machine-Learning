using System;
using System.IO;

namespace IdManagement
{
    public class IdManager
    {
        //Properties
        private int _lastID { get; set; }
        private object newIdLock = new object();

        //Constructors
        public IdManager()
        {
            //Start for now with zero. Later it would be better to load externally
            //so it is not lost during reset.
            _lastID = 0;
        }

        //Methods
        public int GenerateId()
        {
            lock(newIdLock)
            {
                _lastID++;
                return _lastID; 
            }
        }
    }
}
