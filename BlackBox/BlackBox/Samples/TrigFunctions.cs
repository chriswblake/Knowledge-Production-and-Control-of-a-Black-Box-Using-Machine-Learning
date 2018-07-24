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
            AddInput("x");

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
            o["sin"] = Math.Sin(Convert.ToDouble(i["x"]));
            o["cos"] = Math.Cos(Convert.ToDouble(i["x"]));
            o["tan"] = Math.Tan(Convert.ToDouble(i["x"]));
        }
    }
}
