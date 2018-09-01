using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace KnowledgeProduction
{
    public abstract partial class KnowInstance : IComparable
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
    
        //IComparable
        public int CompareTo(object obj)
        {
            KnowInstance that = (KnowInstance) obj;
            if (this.ID > that.ID)
                return 1;
            if (this.ID < that.ID)
                return -1;
            else
                return 0;
        }
    }

    public abstract partial class KnowInstance
    {
        //Display Tools
        public virtual string ContentToString()
        {
            return this.Content.ToString();
        }
        static public string ToHtmlTable(List<KnowInstance> knowInstances)
        {
            string s = "";
            s += "<table border='1'>\n";

            //Header row
            s += "<tr>";
            s += "<th>ID</th>";
            s += "<th>Content</th>";
            s += "</tr>\n";

            //Sort knowInstances by ID
            knowInstances = knowInstances.OrderBy(p => p.ID).ToList();

            //Content rows
            foreach (KnowInstance k in knowInstances)
            {
                s += "<tr>\n";
                //Show ID
                s += "<td>" + k.ID.ToString() + "</td>\n";
                s += "<td>" + k.ContentToString() + "</td>\n";
                s += "</tr>\n";
            }

            s += "</table>\n";

            return s;
        }
    }
}
