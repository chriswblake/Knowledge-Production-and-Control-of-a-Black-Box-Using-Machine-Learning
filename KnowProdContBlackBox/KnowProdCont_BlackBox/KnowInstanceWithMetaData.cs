using System;
using System.Collections.Generic;
using System.Text;
using KnowledgeProduction;
using IdManagement;

namespace KnowProdContBlackBox
{
    public class KnowInstanceWithMetaData : IComparable
    {
        //Fields
        public IdManager InnerIdManager { get; private set; }
        public KnowInstance InnerKnowInstance { get; private set; }

        //Properties
        public int ID
        {
            get { return InnerKnowInstance.ID; }
        }
        public string Name
        {
            get
            {
                return InnerIdManager.GetName(InnerKnowInstance.ID);
            }
            set
            {
                InnerIdManager.SetName(InnerKnowInstance.ID, value);
            }
        }
        public string IdAndName
        {
            get
            {
                return string.Format("({0}) {1}", InnerKnowInstance.ID, InnerIdManager.GetName(InnerKnowInstance.ID));
            }
        }
        public string Description
        {
            get
            {
                return InnerIdManager.GetDescription(InnerKnowInstance.ID);
            }
            set
            {
                InnerIdManager.SetDescription(InnerKnowInstance.ID, value);
            }
        }
        public string AdditionalNotes
        {
            get
            {
                return InnerIdManager.GetAdditionalNotes(InnerKnowInstance.ID);
            }
            set
            {
                InnerIdManager.SetAdditionalNotes(InnerKnowInstance.ID, value);
            }
        }

        //Constructor
        public KnowInstanceWithMetaData(KnowInstance knowInstance, IdManager idManager)
        {
            //Check parameters are not null
            if (knowInstance == null)
                throw new ArgumentNullException("knowInstance");
            if (idManager == null)
                throw new ArgumentNullException("idManager");

            //Save
            this.InnerKnowInstance = knowInstance;
            this.InnerIdManager = idManager;
        }

        //Methods
        public override string ToString()
        {
            return InnerKnowInstance.ToString();
        }
        public int CompareTo(object obj)
        {
            KnowInstanceWithMetaData that = (KnowInstanceWithMetaData)obj;
            if (this.ID > that.ID)
                return 1;
            if (this.ID < that.ID)
                return -1;
            else
                return 0;
        }
        public override int GetHashCode()
        {
            return InnerKnowInstance.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return (this.GetHashCode() == obj.GetHashCode());
        }
    }
}
