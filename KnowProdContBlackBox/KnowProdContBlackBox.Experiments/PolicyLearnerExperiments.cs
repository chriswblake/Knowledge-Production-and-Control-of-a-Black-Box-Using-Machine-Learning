using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;
using BlackBoxModeling;
using Discretization;
using static Discretization.DataGeneration;
using KnowledgeProduction;
using IdManagement;
using RLDT;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace KnowProdContBlackBox.Experiments
{
    public class PolicyLearnerExperiments : Experiment
    {
        [Theory]
        [InlineData(500)]
        [InlineData(1000)]
        [InlineData(1500)]
        public void LogicOperations(int iterations)
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
            PolicyLearner policyLearner = new PolicyLearner(interpreter, idManager);
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

            //Get Discretizers and Producers
            var disc1 = discBlackBox.Discretizers["bool1"];
            var disc2 = discBlackBox.Discretizers["bool2"];
            var discand = discBlackBox.Discretizers["and"];
            var discor = discBlackBox.Discretizers["or"];
            var discxor = discBlackBox.Discretizers["xor"];
            var prod1 = prodBlackBox.Producers["bool1"];
            var prod2 = prodBlackBox.Producers["bool2"];
            var prodand = prodBlackBox.Producers["and"];
            var prodor = prodBlackBox.Producers["or"];
            var prodxor = prodBlackBox.Producers["xor"];
            Policy policyand = policyLearner.Policies["and"];
            Policy policyor = policyLearner.Policies["or"];
            Policy policyxor = policyLearner.Policies["xor"];

            //Check if training was sucessfull
            Assert.Equal(4, disc1.Bins.Count);
            Assert.Equal(4, disc2.Bins.Count);
            Assert.Equal(4, discand.Bins.Count);
            Assert.Equal(4, discor.Bins.Count);
            Assert.Equal(4, discxor.Bins.Count);
            Assert.Equal(4, discxor.Bins.Count);
            Assert.InRange(prod1.KnowInstances.Count, 8, 10);
            Assert.InRange(prod2.KnowInstances.Count, 8, 10);
            Assert.InRange(prodand.KnowInstances.Count, 8, 10);
            Assert.InRange(prodor.KnowInstances.Count, 8, 10);
            Assert.InRange(prodxor.KnowInstances.Count, 8, 10);

            //Name Entities
            #region bool1
            KnowInstance bool1_On = prod1.Get(disc1.GetBin(5.0).BinID);
            KnowInstance bool1_Off = prod1.Get(disc1.GetBin(0.0).BinID);
            KnowInstance bool1_StayOff = prod1.Get(bool1_Off, bool1_Off);
            KnowInstance bool1_StayOn = prod1.Get(bool1_On, bool1_On);
            KnowInstance bool1_SwitchOff = prod1.Get(bool1_On, bool1_Off);
            KnowInstance bool1_SwitchOn = prod1.Get(bool1_Off, bool1_On);
            idManager.SetName(bool1_On.ID, "True");
            idManager.SetName(bool1_Off.ID, "False");
            idManager.SetName(bool1_StayOff.ID, "Stay False");
            idManager.SetName(bool1_StayOn.ID, "Stay True");
            idManager.SetName(bool1_SwitchOff.ID, "True to False");
            idManager.SetName(bool1_SwitchOn.ID, "False to True");
            #endregion
            #region bool2
            KnowInstance bool2_On = prod2.Get(disc2.GetBin(5.0).BinID);
            KnowInstance bool2_Off = prod2.Get(disc2.GetBin(0.0).BinID);
            KnowInstance bool2_StayOff = prod2.Get(bool2_Off, bool2_Off);
            KnowInstance bool2_StayOn = prod2.Get(bool2_On, bool2_On);
            KnowInstance bool2_SwitchOff = prod2.Get(bool2_On, bool2_Off);
            KnowInstance bool2_SwitchOn = prod2.Get(bool2_Off, bool2_On);
            idManager.SetName(bool2_On.ID, "True");
            idManager.SetName(bool2_Off.ID, "False");
            idManager.SetName(bool2_StayOff.ID, "Stay False");
            idManager.SetName(bool2_StayOn.ID, "Stay True");
            idManager.SetName(bool2_SwitchOff.ID, "True to False");
            idManager.SetName(bool2_SwitchOn.ID, "False to True");
            #endregion
            #region booland
            KnowInstance and_On = prodand.Get(discand.GetBin(5.0).BinID);
            KnowInstance and_Off = prodand.Get(discand.GetBin(0.0).BinID);
            KnowInstance and_StayOff = prodand.Get(and_Off, and_Off);
            KnowInstance and_StayOn = prodand.Get(and_On, and_On);
            KnowInstance and_SwitchedOff = prodand.Get(and_On, and_Off);
            KnowInstance and_SwitchedOn = prodand.Get(and_Off, and_On);
            idManager.SetName(and_On.ID, "True");
            idManager.SetName(and_Off.ID, "False");
            idManager.SetName(and_StayOff.ID, "Stay False");
            idManager.SetName(and_StayOn.ID, "Stay True");
            idManager.SetName(and_SwitchedOff.ID, "True to False");
            idManager.SetName(and_SwitchedOn.ID, "False to True");
            #endregion
            #region boolor
            KnowInstance or_On = prodor.Get(discor.GetBin(5.0).BinID);
            KnowInstance or_Off = prodor.Get(discor.GetBin(0.0).BinID);
            KnowInstance or_StayOff = prodor.Get(or_Off, or_Off);
            KnowInstance or_StayOn = prodor.Get(or_On, or_On);
            KnowInstance or_SwitchedOff = prodor.Get(or_On, or_Off);
            KnowInstance or_SwitchedOn = prodor.Get(or_Off, or_On);
            idManager.SetName(or_On.ID, "True");
            idManager.SetName(or_Off.ID, "False");
            idManager.SetName(or_StayOff.ID, "Stay False");
            idManager.SetName(or_StayOn.ID, "Stay True");
            idManager.SetName(or_SwitchedOff.ID, "True to False");
            idManager.SetName(or_SwitchedOn.ID, "False to True");
            #endregion
            #region boolxor
            KnowInstance xor_On = prodxor.Get(discxor.GetBin(5.0).BinID);
            KnowInstance xor_Off = prodxor.Get(discxor.GetBin(0.0).BinID);
            KnowInstance xor_StayOff = prodxor.Get(xor_Off, xor_Off);
            KnowInstance xor_StayOn = prodxor.Get(xor_On, xor_On);
            KnowInstance xor_SwitchedOff = prodxor.Get(xor_On, xor_Off);
            KnowInstance xor_SwitchedOn = prodxor.Get(xor_Off, xor_On);
            idManager.SetName(xor_On.ID, "True");
            idManager.SetName(xor_Off.ID, "False");
            idManager.SetName(xor_StayOff.ID, "Stay False");
            idManager.SetName(xor_StayOn.ID, "Stay True");
            idManager.SetName(xor_SwitchedOff.ID, "True to False");
            idManager.SetName(xor_SwitchedOn.ID, "False to True");
            #endregion

            //Convert policies to html and save to file
            string htmlTree = HtmlTools.ToHtml(policyLearner, idManager);
            string savePath = Path.Combine(this.ResultsDir, "LogicOperations", iterations + ".html");
            File.WriteAllText(savePath, htmlTree);

            return;
        }

        [Theory]
        //[InlineData(5)]
        //[InlineData(10)]
        //[InlineData(15)]
        //[InlineData(20)]
        //[InlineData(25)]
        //[InlineData(30)]
        //[InlineData(35)]
        [InlineData(40)]
        public void TrigFunctions(int passes)
        {
            double angleInterval = 1;

            List<double> angles = new List<double> {};
            for (double r = 0; r < 180; r += angleInterval)
                angles.Add(r);
            IdManager idManager = new IdManager();
            BlackBox trigBlackBox = new BlackBoxModeling.Samples.TrigFunctions() { TimeInterval_ms = 10 };
            DiscreteBlackBox discBlackBox = new DiscreteBlackBox(trigBlackBox, idManager);
            ProducerBlackBox prodBlackBox = new ProducerBlackBox(discBlackBox, idManager);
            Interpreter interpreter = new Interpreter(prodBlackBox) { MemorySize = passes * 2 };
            PolicyLearner policyLearner = new PolicyLearner(interpreter, idManager);
            Random rand = new Random();
            trigBlackBox.Start();

            #region Train
            for (int i = 0; i < passes; i++)
            {
                //Change input
                List<double> angles_noisy = GenerateNoisyData(angles, 0.001, 1).OrderBy(p => rand.NextDouble()).ToList();
                foreach(double angle in angles_noisy)
                {
                    for (int j = 0; j < 1; j++)
                    { 
                        trigBlackBox.Input["angle"] = angle;

                        //Wait until next sample time
                        Thread.Sleep(trigBlackBox.TimeInterval_ms);
                    }
                }
            }
            #endregion

            //Get Discretizers and Producers
            var discAngle = discBlackBox.Discretizers["angle"];
            var discSin = discBlackBox.Discretizers["sin"];
            var discCos = discBlackBox.Discretizers["cos"];
            var discTan = discBlackBox.Discretizers["tan"];
            var prodAngle = prodBlackBox.Producers["angle"];
            var prodSin = prodBlackBox.Producers["sin"];
            var prodCos = prodBlackBox.Producers["cos"];
            var prodTan = prodBlackBox.Producers["tan"];
            Policy policySin = policyLearner.Policies["sin"];
            Policy policyCos = policyLearner.Policies["cos"];
            Policy policyTan = policyLearner.Policies["tan"];

            //Check if training was sucessfull
            //Assert.Equal(4, disc1.Bins.Count);
            //Assert.Equal(4, disc2.Bins.Count);
            //Assert.Equal(4, discand.Bins.Count);
            //Assert.Equal(4, discor.Bins.Count);
            //Assert.Equal(4, discxor.Bins.Count);
            //Assert.Equal(4, discxor.Bins.Count);
            //Assert.InRange(prod1.KnowInstances.Count, 8, 10);
            //Assert.InRange(prod2.KnowInstances.Count, 8, 10);
            //Assert.InRange(prodand.KnowInstances.Count, 8, 10);
            //Assert.InRange(prodor.KnowInstances.Count, 8, 10);
            //Assert.InRange(prodxor.KnowInstances.Count, 8, 10);

            //Name Entities by their average
            foreach(Bin theBin in discAngle.Bins)
                idManager.SetName(theBin.BinID, theBin.Average.ToString("N2"));
            foreach (Bin theBin in discSin.Bins)
                idManager.SetName(theBin.BinID, theBin.Average.ToString("N2"));
            foreach (Bin theBin in discCos.Bins)
                idManager.SetName(theBin.BinID, theBin.Average.ToString("N2"));
            foreach (Bin theBin in discTan.Bins)
                idManager.SetName(theBin.BinID, theBin.Average.ToString("N2"));

            //Calculate predicted values and error
            Dictionary<double, Dictionary<string, double>> compResults = new Dictionary<double, Dictionary<string, double>>();
            Dictionary<string, double> trigEntryTemplate = new Dictionary<string, double>();
            trigEntryTemplate.Add("sin", double.NaN);
            trigEntryTemplate.Add("cos", double.NaN);
            trigEntryTemplate.Add("tan", double.NaN);
            trigEntryTemplate.Add("sinPred", double.NaN);
            trigEntryTemplate.Add("cosPred", double.NaN);
            trigEntryTemplate.Add("tanPred", double.NaN);
            trigEntryTemplate.Add("sinError", double.NaN);
            trigEntryTemplate.Add("cosError", double.NaN);
            trigEntryTemplate.Add("tanError", double.NaN);
            for (double angle=0; angle <= 180; angle += angleInterval)
            {
                //Create entry in dictionaries
                compResults.Add(angle, new Dictionary<string, double>(trigEntryTemplate));
                double radians = angle / 180 * Math.PI;

                //Get real values
                compResults[angle]["sin"] = Math.Sin(radians);
                compResults[angle]["cos"] = Math.Cos(radians);
                compResults[angle]["tan"] = Math.Tan(radians);

                //Convert angle to knowledge instance
                Bin binAngle = discAngle.GetBin(angle);
                KnowInstance ki = prodAngle.Get(binAngle.BinID);

                #region Get predicted values
                DataVector dv = new DataVector(new string[] {"angle"}, new object[] {ki});
                //Sin
                try
                {
                    KnowInstanceValue predKi_sin = (KnowInstanceValue) ((KnowInstanceWithMetaData)policySin.Classify_ByPolicy(dv, false)).InnerKnowInstance;
                    Bin predBinSin = (Bin)predKi_sin.Content;
                    compResults[angle]["sinPred"] = predBinSin.Average;
                }
                catch { }
                //Cos
                try
                {
                    KnowInstanceValue predKi_cos = (KnowInstanceValue)((KnowInstanceWithMetaData)policyCos.Classify_ByPolicy(dv, false)).InnerKnowInstance;
                    Bin predBinCos = (Bin)predKi_cos.Content;
                    compResults[angle]["cosPred"] = predBinCos.Average;
                }
                catch { }
                //Tan
                try
                {
                    KnowInstanceValue predKi_tan = (KnowInstanceValue)((KnowInstanceWithMetaData)policyTan.Classify_ByPolicy(dv, false)).InnerKnowInstance;
                    Bin predBinTan = (Bin)predKi_tan.Content;
                    compResults[angle]["tanPred"] = predBinTan.Average;
                }
                catch { }
                #endregion

                //Calculate error
                compResults[angle]["sinError"] = compResults[angle]["sin"] - compResults[angle]["sinPred"];
                compResults[angle]["cosError"] = compResults[angle]["cos"] - compResults[angle]["cosPred"];
                compResults[angle]["tanError"] = compResults[angle]["tan"] - compResults[angle]["tanPred"];
            }

            //Save error results to csv
            List<string> errorResults = new List<string>();
            errorResults.Add("angle,sin,cos,tan,Predicted Sin,Predicted Cos,Predicted Tan, Error Sin, Error Cos, Error Tan");
            foreach(var prediction in compResults)
            {
                double angle = prediction.Key;
                string row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                    angle,
                    compResults[angle]["sin"],
                    compResults[angle]["cos"],
                    compResults[angle]["tan"],
                    
                    compResults[angle]["sinPred"],
                    compResults[angle]["cosPred"],
                    compResults[angle]["tanPred"],

                    compResults[angle]["sinError"],
                    compResults[angle]["cosError"],
                    compResults[angle]["tanError"]
                    );
                errorResults.Add(row);
            }
            File.WriteAllLines(SavePath(passes + "_ErrorResults.csv"), errorResults.ToArray());

            //Save training results to csv
            File.WriteAllLines(SavePath(passes + "_angle.csv"), ToStringArray(prodAngle.KnowInstances.Values.ToList(), idManager));
            File.WriteAllLines(SavePath(passes + "_sin.csv"), ToStringArray(prodSin.KnowInstances.Values.ToList(), idManager));
            File.WriteAllLines(SavePath(passes + "_cos.csv"), ToStringArray(prodCos.KnowInstances.Values.ToList(), idManager));
            File.WriteAllLines(SavePath(passes + "_tan.csv"), ToStringArray(prodTan.KnowInstances.Values.ToList(), idManager));

            //Convert policies to html and save to file
            string htmlTree = HtmlTools.ToHtml(policyLearner, idManager);
            File.WriteAllText(SavePath(passes + "_decision_tree.html"), htmlTree);

            return;
        }

        public string[] ToStringArray(List<KnowInstance> knowInstances, IdManager idManager)
        {
            List<string> csv = new List<string>();
            csv.Add("ID,Name,Content");
            foreach(KnowInstance ki in knowInstances)
            {
                KnowInstanceWithMetaData kim = new KnowInstanceWithMetaData(ki, idManager);
                csv.Add(string.Format("{0},\"{1}\",\"{2}\"", kim.ID, kim.Name, ki.ContentToString()));
            }

            return csv.ToArray();
        }
        private string SavePath(string fileName)
        {
            return Path.Combine(this.ResultsDir, fileName);
        }
        
    }
}
