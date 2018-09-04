using IdManagement;
using KnowledgeProduction;
using RLDT;
using System.Collections.Generic;
using System.Linq;

namespace KnowProdContBlackBox
{
    static public class HtmlTools
    {
        static public string ToHtml(PolicyLearner policyLearner, IdManager idManager)
        {
            //Generate table of vocabulary
            string htmlVocabStyle = VocabularyTableStyle();
            string htmlVocab = "<table>\n";
            htmlVocab += "<tr>\n";
            foreach(string ioName in policyLearner.InputNames.Concat(policyLearner.OutputNames))
            {
                htmlVocab += "<td style='vertical-align:top;'>\n";
                htmlVocab += ToVocabularyHtmlTable(policyLearner.GetVocabulary(ioName), idManager, ioName) + "\n";
                htmlVocab += "</td>\n\n";
            }
            htmlVocab += "</tr>\n";
            htmlVocab += "</table>";

            //Generate html trees for each policy/output
            var treeSettingsConversion = new RLDT.DecisionTree.TreeSettings() { ShowBlanks = true, ShowSubScores = false };
            var treeSettingsDisplay = new RLDT.DecisionTree.TreeNode.TreeDisplaySettings() {
                IncludeDefaultTreeStyling = false,
                ValueDisplayProperty = "IdName",
                LabelDisplayProperty = "IdName" };
            string htmlTreeStyle = RLDT.DecisionTree.TreeNode.DefaultStyling;
            string htmlTrees = "";
            foreach (var p in policyLearner.Policies)
            {
                string policyName = p.Key; //OutputName
                Policy thePolicy = p.Value;
                htmlTrees += "</br>\n";
                htmlTrees += "</br>\n";
                htmlTrees += thePolicy.ToDecisionTree(treeSettingsConversion).ToHtmlTree(treeSettingsDisplay, policyName);
            }

            //Combine together
            string html = "";
            html += htmlVocabStyle;
            html += htmlTreeStyle;
            html += htmlVocab;
            html += htmlTrees;

            return html;
        }


        static public string ToVocabularyHtmlTable(List<KnowInstance> knowInstances, string title)
        {
            return ToVocabularyHtmlTable(knowInstances, null, title);
        }
        static public string ToVocabularyHtmlTable(List<KnowInstance> knowInstances, IdManager idManager, string title)
        {
            string s = "";
            s += "<div class='Vocabulary'>\n";
            s += "<table border='1'>\n";

            //Title
            s += "<tr><th colspan='1000' class='title'>" + title+"</th></tr>\n";

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
            s += "</div>\n";

            return s;
        }
        static public string VocabularyTableStyle()
        {
            return @"
    <style>
    div.Vocabulary table
    {
        border-collapse: collapse;
        padding: 0px;
        margin: 0px;
        font: 8pt arial, sans-serif;
        border: 1px solid #AAAAAA
    }
    div.Vocabulary table .title {
	    border: 1px solid #888888;
	    background-color: #CCCCCC;
    }
    div.Vocabulary table th {
	    border: 1px solid #AAAAAA;
            font-weight: bold;
	    padding: 3px;
    }
    div.Vocabulary table td{
	    border: 1px solid #AAAAAA;
	    padding: 3px;
    }
    </style>
        ";
        }
    }
    
}
