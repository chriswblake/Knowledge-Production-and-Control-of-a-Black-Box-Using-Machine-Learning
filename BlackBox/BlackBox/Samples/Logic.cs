using System;
using System.Collections.Generic;
using System.Text;
using BlackBoxModeling;
using System.Linq;

namespace BlackBoxModeling.Samples
{
    public class Logic : BlackBox
    {
        //Constructors
        public Logic()
        {
            //Set Name
            this.Name = "Logic Functions";

            //Define inputs
            AddInput("bool1");
            AddInput("bool2");

            //Define outputs
            AddOutput("and");
            AddOutput("or");
            AddOutput("xor");
        }

        //Methods
        public override void Run()
        {
            //Convert input to double
            double bool1 = Convert.ToDouble(Input["bool1"]);
            double bool2 = Convert.ToDouble(Input["bool2"]);
            var o = Output;

            //Define relationship between outputs and inputs
            //and
            if ((bool1 >= 4.5) && (bool2 >= 4.5))
                o["and"] = 5.0;
            else
                o["and"] = 0.0;

            //or
            if ((bool1 >= 4.5) || (bool2 >= 4.5))
                o["or"] = 5.0;
            else
                o["or"] = 0.0;

            //xor
            if ((bool1 >= 4.5) ^ (bool2 >= 4.5))
                o["xor"] = 5.0;
            else
                o["xor"] = 0.0;
        }

    }
}
