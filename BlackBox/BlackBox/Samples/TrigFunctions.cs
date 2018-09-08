using System;
using System.Collections.Generic;
using System.Text;
using BlackBoxModeling;

namespace BlackBoxModeling.Samples
{
    public class TrigFunctions : BlackBox
    {
        //Constructors
        public TrigFunctions()
        {
            //Set Name
            this.Name = "Trig Functions";

            //Define inputs
            AddInput("angle");

            //Define outputs
            AddOutput("sin");
            AddOutput("cos");
            AddOutput("tan");

            //Start black box
            //this.Start();
        }

        //Methods
        public override void Run()
        {
            var i = Input;
            var o = Output;

            //Define relationship between outputs and inputs
            double radians = Convert.ToDouble(i["angle"])/180*Math.PI;
            o["sin"] = Math.Sin(radians);
            o["cos"] = Math.Cos(radians);
            o["tan"] = Math.Tan(radians);
        }
    }
}
