using System;
using System.IO;

namespace IdManager
{
    public static class IdManager
    {
        //Properties
        private static int LastUsedID { get; set; }
        private static object newIdLock = new object();

        //Constructors
        static IdManager()
        {
            //Load previous id information
            LastUsedID = 0;
        }

        //Methods
        public static int GetNewId()
        {
            lock(newIdLock)
            {
                LastUsedID++;
                return LastUsedID; 
            }
        }
    }
}
