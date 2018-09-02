using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using KnowledgeProduction;
using IdManagement;

namespace KnowProdContBlackBox
{
    static public class HtmlTools
    {
        static public string ToHtmlTable(List<KnowInstance> knowInstances)
        {
            return ToHtmlTable(knowInstances, null);
        }
        static public string ToHtmlTable(List<KnowInstance> knowInstances, IdManager idManager)
        {
            string s = "";
            s += "<table border='1'>\n";

            //Header row
            s += "<tr>";
            s += "<th>ID</th>";
            if (idManager != null)
                s += "<th>Name</th>";
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
                if (idManager != null)
                {
                    KnowInstanceWithMetaData kim = new KnowInstanceWithMetaData(k, idManager);
                    s += "<td>" + kim.Name + "</td>\n";
                }
                s += "<td>" + k.ContentToString() + "</td>\n";
                s += "</tr>\n";
            }

            s += "</table>\n";

            return s;
        }
    }
}
