using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using Xunit;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using BlackBoxModeling;
using Discretization;
using IdManagement;
using static Discretization.DataGeneration;
using System.Linq;
using KnowledgeProduction;

namespace KnowProdContBlackBox.Experiments
{
    public class KnowledgeProductionExperiments : Experiment
    {
        //[Theory]
        //[InlineData(100)]
        //[InlineData(200)]
        //[InlineData(300)]
        //[InlineData(400)]
        //[InlineData(500)]
        public void BinarySignal(int iterations)
        {
            List<double> bool1 = new List<double> {
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
            };
            List<double> bool2 = new List<double> {
                0.0,
                0.0,
                0.0,
                    5.0,
                    5.0,
                    5.0,
            };
            IdManager idManager = new IdManager();
            BlackBox logicBlackBox = new BlackBoxModeling.Samples.Logic() { TimeInterval_ms = 10 };
            DiscreteBlackBox discBlackBox = new DiscreteBlackBox(logicBlackBox, idManager);
            ProducerBlackBox prodBlackBox = new ProducerBlackBox(discBlackBox, idManager);
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
            var prod1 = prodBlackBox.Producers["bool1"];
            var prod2 = prodBlackBox.Producers["bool2"];

            //Check if training was sucessfull
            Assert.Equal(4, disc1.Bins.Count);
            Assert.Equal(4, disc2.Bins.Count);
            Assert.Equal(8, prod1.KnowInstances.Count);
            Assert.Equal(8, prod2.KnowInstances.Count);

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

            //Convert to html tables
            string html1 = HtmlTools.ToVocabularyHtmlTable(prod1.KnowInstances.Values.ToList(), idManager, "bool1");
            string html2 = HtmlTools.ToVocabularyHtmlTable(prod2.KnowInstances.Values.ToList(), idManager, "bool2");

            //Save resulting html files to disk
            File.WriteAllText(SavePath(iterations+"_bool1.html"), html1);
            File.WriteAllText(SavePath(iterations+"_bool2.html"), html2);
        }

        //[Theory]
        //[InlineData(500)]
        //[InlineData(600)]
        //[InlineData(700)]
        //[InlineData(800)]
        //[InlineData(900)]
        public void CategoricalSignal(int iterations)
        {
            List<double> cat1 = new List<double> {
                0.0,
                0.0,
                0.0,
                    1.0,
                    1.0,
                    1.0,
                        2.0,
                        2.0,
                        2.0,
                            3.0,
                            3.0,
                            3.0,
                                4.0,
                                4.0,
                                4.0,
                                    5.0,
                                    5.0,
                                    5.0,
                                4.0,
                                4.0,
                                4.0,
                            3.0,
                            3.0,
                            3.0,
                        2.0,
                        2.0,
                        2.0,
                    1.0,
                    1.0,
                    1.0,
                0.0,
                0.0,
                0.0,
            };

            IdManager idManager = new IdManager();
            BlackBox logicBlackBox = new BlackBoxModeling.Samples.Logic() { TimeInterval_ms = 10 };
            DiscreteBlackBox discBlackBox = new DiscreteBlackBox(logicBlackBox, idManager);
            ProducerBlackBox prodBlackBox = new ProducerBlackBox(discBlackBox, idManager);
            Random rand = new Random();
            logicBlackBox.Start();

            #region Train
            for (int i = 0; i < iterations; i++)
            {
                //Change input
                double bool1_crisp = cat1[i % cat1.Count];
                logicBlackBox.Input["bool1"] = GenerateNoisyValue(rand, bool1_crisp, 0.001);
                logicBlackBox.Input["bool2"] = 0;

                //Wait until next sample time
                Thread.Sleep(logicBlackBox.TimeInterval_ms);
            }
            #endregion

            //Get Discretizers and Producers
            var discCat = discBlackBox.Discretizers["bool1"];
            var prodCat = prodBlackBox.Producers["bool1"];

            //Check if training was sucessfull
            Assert.Equal(8, discCat.Bins.Count);
            Assert.Equal(24, prodCat.KnowInstances.Count);

            //Name the entities
            #region Categorical
            KnowInstance ki_0 = prodCat.Get(discCat.GetBin(0.0).BinID);
            KnowInstance ki_1 = prodCat.Get(discCat.GetBin(1.0).BinID);
            KnowInstance ki_2 = prodCat.Get(discCat.GetBin(2.0).BinID);
            KnowInstance ki_3 = prodCat.Get(discCat.GetBin(3.0).BinID);
            KnowInstance ki_4 = prodCat.Get(discCat.GetBin(4.0).BinID);
            KnowInstance ki_5 = prodCat.Get(discCat.GetBin(5.0).BinID);

            KnowInstance ki_0_0 = prodCat.Get(ki_0, ki_0);
            KnowInstance ki_1_1 = prodCat.Get(ki_1, ki_1);
            KnowInstance ki_2_2 = prodCat.Get(ki_2, ki_2);
            KnowInstance ki_3_3 = prodCat.Get(ki_3, ki_3);
            KnowInstance ki_4_4 = prodCat.Get(ki_4, ki_4);
            KnowInstance ki_5_5 = prodCat.Get(ki_5, ki_5);

            KnowInstance ki_0_1 = prodCat.Get(ki_0, ki_1);
            KnowInstance ki_1_2 = prodCat.Get(ki_1, ki_2);
            KnowInstance ki_2_3 = prodCat.Get(ki_2, ki_3);
            KnowInstance ki_3_4 = prodCat.Get(ki_3, ki_4);
            KnowInstance ki_4_5 = prodCat.Get(ki_4, ki_5);

            KnowInstance ki_5_4 = prodCat.Get(ki_5, ki_4);
            KnowInstance ki_4_3 = prodCat.Get(ki_4, ki_3);
            KnowInstance ki_3_2 = prodCat.Get(ki_3, ki_2);
            KnowInstance ki_2_1 = prodCat.Get(ki_2, ki_1);
            KnowInstance ki_1_0 = prodCat.Get(ki_1, ki_0);

            idManager.SetName(ki_0.ID, "0.0");
            idManager.SetName(ki_1.ID, "1.0");
            idManager.SetName(ki_2.ID, "2.0");
            idManager.SetName(ki_3.ID, "3.0");
            idManager.SetName(ki_4.ID, "4.0");
            idManager.SetName(ki_5.ID, "5.0");

            idManager.SetName(ki_0_0.ID, "0.0 to 0.0");
            idManager.SetName(ki_1_1.ID, "1.0 to 1.0");
            idManager.SetName(ki_2_2.ID, "2.0 to 2.0");
            idManager.SetName(ki_3_3.ID, "3.0 to 3.0");
            idManager.SetName(ki_4_4.ID, "4.0 to 4.0");
            idManager.SetName(ki_5_5.ID, "5.0 to 5.0");

            idManager.SetName(ki_0_1.ID, "0.0 to 1.0");
            idManager.SetName(ki_1_2.ID, "1.0 to 2.0");
            idManager.SetName(ki_2_3.ID, "2.0 to 3.0");
            idManager.SetName(ki_3_4.ID, "3.0 to 4.0");
            idManager.SetName(ki_4_5.ID, "4.0 to 5.0");

            idManager.SetName(ki_5_4.ID, "5.0 to 4.0");
            idManager.SetName(ki_4_3.ID, "4.0 to 3.0");
            idManager.SetName(ki_3_2.ID, "3.0 to 2.0");
            idManager.SetName(ki_2_1.ID, "2.0 to 1.0");
            idManager.SetName(ki_1_0.ID, "1.0 to 0.0");
            #endregion

            //Convert to html tables
            string htmlCat = HtmlTools.ToVocabularyHtmlTable(prodCat.KnowInstances.Values.ToList(), idManager, "Categorical");

            //Save resulting html files to disk
            File.WriteAllText(SavePath(iterations + "_bool1.html"), htmlCat);

            ///Theoretical knowledge instances (8 bins, 16 combos) 24 total
            ///(0.0)
            ///(1.0)
            ///(2.0)
            ///(3.0)
            ///(4.0)
            ///(5.0)
            /// 
            ///(0.0; 0.0)
            ///(0.0; 1.0)
            ///(1.0; 1.0)
            ///(1.0; 2.0)
            ///(2.0; 2.0)
            ///(2.0; 3.0)
            ///(3.0; 3.0)
            ///(3.0; 4.0)
            ///(4.0; 4.0)
            ///(4.0; 5.0)
            ///(5.0; 5.0)
            ///(5.0; 4.0)
            ///(4.0; 3.0)
            ///(3.0; 2.0)
            ///(2.0; 1.0)
            ///(1.0; 0.0)
        }

        //Methods
        private string SavePath(string fileName)
        {
            return Path.Combine(this.ResultsDir, fileName);
        }
    }
}
