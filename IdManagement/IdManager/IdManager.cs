using System;
using System.Collections.Generic;
using System.IO;

namespace IdManagement
{
    public class IdManager
    {
        //Properties
        private int _lastID { get; set; }
        private object newIdLock = new object();
        private Dictionary<int, IdMetadata> metadata = new Dictionary<int, IdMetadata>();

        //Constructors
        public IdManager()
        {
            //Start for now with zero. Later it would be better to load externally
            //so it is not lost during reset.
            _lastID = 0;
        }

        //Methods - IDs
        public int GenerateId()
        {
            lock(newIdLock)
            {
                _lastID++;
                return _lastID; 
            }
        }

        //Methods - Exta metadata
        private IdMetadata GetOrCreateMetadata(int id)
        {
            if (!metadata.ContainsKey(id))
                metadata[id] = new IdMetadata();
            return metadata[id];
        }
        public void SetName(int id, string name)
        {
            GetOrCreateMetadata(id).Name = name;
        }
        public void SetDescription(int id, string description)
        {
            GetOrCreateMetadata(id).Description = description;
        }
        public void SetAdditionalNotes(int id, string additionalNotes)
        {
            GetOrCreateMetadata(id).AdditionalNotes = additionalNotes;
        }
        public string GetName(int id)
        {
            return GetOrCreateMetadata(id).Name;
        }
        public string GetDescription(int id)
        {
            return GetOrCreateMetadata(id).Description;
        }
        public string GetAdditionalNotes(int id)
        {
            return GetOrCreateMetadata(id).AdditionalNotes;
        }
    }
}
