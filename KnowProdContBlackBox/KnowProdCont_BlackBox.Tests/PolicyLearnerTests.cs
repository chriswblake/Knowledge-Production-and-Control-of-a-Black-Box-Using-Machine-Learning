using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using IdManagement;
using BlackBoxModeling;
using KnowledgeProduction;
using static Discretization.DataGeneration;
using System.Threading;
using RLDT;
using System.IO;
using Discretization;

namespace KnowProdContBlackBox.Tests
{
    public class PolicyLearnerTests
    {
        [Fact]
        public void Constructor()
        {
            IdManager idManager = new IdManager();
            BlackBox logicBlackBox = new BlackBoxModeling.Samples.Logic() { TimeInterval_ms = 10 };
            DiscreteBlackBox discBlackBox = new DiscreteBlackBox(logicBlackBox, idManager);
            ProducerBlackBox prodBlackBox = new ProducerBlackBox(discBlackBox, idManager);
            Interpreter interpreter = new Interpreter(prodBlackBox);
            PolicyLearner policyLearner = new PolicyLearner(interpreter);

            Assert.Equal(3, policyLearner.Policies.Count);
        }

        [Theory]
        [InlineData(1000)]
        public void Learn_OnOffPattern_Count4(int iterations)
        {
            List<double> bool1 = new List<double> {
                0.0,
                0.0,
                0.0,
                0.0,

                0.0,
                0.0,
                0.0,
                0.0,

                    5.0,
                    5.0,
                    5.0,
                    5.0,

                    5.0,
                    5.0,
                    5.0,
                    5.0,
            };
            List<double> bool2 = new List<double> {
                0.0,
                0.0,
                0.0,
                0.0,

                    5.0,
                    5.0,
                    5.0,
                    5.0,

                    5.0,
                    5.0,
                    5.0,
                    5.0,

                0.0,
                0.0,
                0.0,
                0.0,
            };
            IdManager idManager = new IdManager();
            BlackBox logicBlackBox = new BlackBoxModeling.Samples.Logic() { TimeInterval_ms = 10 };
            DiscreteBlackBox discBlackBox = new DiscreteBlackBox(logicBlackBox, idManager);
            ProducerBlackBox prodBlackBox = new ProducerBlackBox(discBlackBox, idManager);
            Interpreter interpreter = new Interpreter(prodBlackBox) { MemorySize = iterations * 2 };
            PolicyLearner policyLearner = new PolicyLearner(interpreter);
            Random rand = new Random();
            logicBlackBox.Start();

            #region Train
            for (int i = 0; i < iterations; i++)
            {
                //Change input
                double bool1_crisp = bool1[i % bool1.Count];
                double bool2_crisp = bool2[i % bool2.Count];
                logicBlackBox.Input["bool1"] = GenerateNoisyValue(rand, bool1_crisp, 0.001);
                logicBlackBox.Input["bool2"] = GenerateNoisyValue(rand, bool2_crisp, 0.001);

                //Wait until next sample time
                Thread.Sleep(logicBlackBox.TimeInterval_ms);
            }
            #endregion

            //Decision Trees (detailed and simple)
            var treeSettingsBlanksSubScores = new RLDT.DecisionTree.TreeSettings() { ShowBlanks = true, ShowSubScores = true };
            var treeSettingsBlanks = new RLDT.DecisionTree.TreeSettings() { ShowBlanks = true, ShowSubScores = false };
            var treeSettingsSimple = new RLDT.DecisionTree.TreeSettings() { ShowBlanks = false, ShowSubScores = false };
            var treeDisplaySettings = new RLDT.DecisionTree.TreeNode.TreeDisplaySettings() { ValueDisplayProperty="ID", LabelDisplayProperty="ID" };
            string htmlTree_BlanksSubScores = policyLearner.Policies["xor"].ToDecisionTree(treeSettingsBlanksSubScores).ToHtmlTree(treeDisplaySettings);
            string htmlTree_Blanks = policyLearner.Policies["xor"].ToDecisionTree(treeSettingsBlanks).ToHtmlTree(treeDisplaySettings);
            string htmlTree_Simple = policyLearner.Policies["xor"].ToDecisionTree(treeSettingsSimple).ToHtmlTree();// treeDisplaySettings);
            string htmlTreeStyle = RLDT.DecisionTree.TreeNode.DefaultStyling;

            //Tables of vocabulary
            string htmlBool1 = "bool1 Vocabulary" + KnowInstance.ToHtmlTable(prodBlackBox.Producers["bool1"].KnowInstances.Values.ToList());
            string htmlBool2 = "bool2 Vocabulary" + KnowInstance.ToHtmlTable(prodBlackBox.Producers["bool2"].KnowInstances.Values.ToList());
            string htmlXor = "xor Vocabulary" + KnowInstance.ToHtmlTable(prodBlackBox.Producers["xor"].KnowInstances.Values.ToList());
            string htmlVocab = string.Format(@"
            <table>
            <tr>
            <td>{0}</td>
            <td>{1}</td>
            <td>{2}</td>
            </tr>
            </table>
            ", htmlBool1, htmlBool2, htmlXor);

            //Save to files
            File.WriteAllText("xorDetailed.html", htmlTreeStyle + htmlVocab + htmlTree_BlanksSubScores);
            File.WriteAllText("xorBlanks.html", htmlTreeStyle + htmlVocab + htmlTree_Blanks);
            File.WriteAllText("xorSimple.html", htmlTreeStyle + htmlVocab + htmlTree_Simple);

            return;

            #region Assert
            //Find equivalent zero and five for both inputs
            var binBool1_0 = discBlackBox.Discretizers["bool1"].GetBin(0.0);
            var binBool1_5 = discBlackBox.Discretizers["bool1"].GetBin(5.0);
            var bool1_0 = prodBlackBox.Producers["bool1"].Get(binBool1_0.BinID);
            var bool1_5 = prodBlackBox.Producers["bool1"].Get(binBool1_5.BinID);

            var binBool2_0 = discBlackBox.Discretizers["bool2"].GetBin(0.0);
            var binBool2_5 = discBlackBox.Discretizers["bool2"].GetBin(5.0);
            var bool2_0 = prodBlackBox.Producers["bool2"].Get(binBool2_0.BinID);
            var bool2_5 = prodBlackBox.Producers["bool2"].Get(binBool2_5.BinID);

            DataVector dataVectorTrue = new DataVector(new string[] {"bool1", "bool2" }, new object[] {bool1_5, bool2_5 });
            DataVector dataVectorFalse = new DataVector(new string[] {"bool1", "bool2" }, new object[] {bool1_0, bool2_5 });

            var result1 = policyLearner.Policies["and"].Classify_ByPolicy(dataVectorTrue);
            var result2 = policyLearner.Policies["and"].Classify_ByPolicy(dataVectorFalse);

            #endregion

        }
    }
}