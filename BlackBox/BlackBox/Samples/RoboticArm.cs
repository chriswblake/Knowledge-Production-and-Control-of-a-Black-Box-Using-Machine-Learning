using System;
using System.Collections.Generic;
using System.Text;

namespace BlackBoxModeling.Samples
{
    public class RoboticArm : BlackBox
    {
        //Fields
        double theta1 = 0;
        double theta2 = 0;

        //Constructors
        public RoboticArm()
        {
            //Set Name
            this.Name = "Robotic Arm";

            //Define inputs
            AddInput("Motor1");
            AddInput("Motor2");

            //Define outputs
            AddOutput("Theta1");
            AddOutput("Theta2");

            //Specify responsiveness
            this.TimeInterval_ms = 250; //i.e. The model needs 0.250 seconds to convert the input into a new output.
        }
        public override void Run()
        {
            var i = Input;
            var o = Output;

            double voltageMotor1 = Convert.ToDouble(i["Motor1"]);
            double voltageMotor2 = Convert.ToDouble(i["Motor2"]);

            //Update theta1 (0 to 360deg)
            theta1 = (theta1 + (0.1 * voltageMotor1)) % 360;

            //Update theta2 (-80 to 90 deg)
            theta2 = theta2 + (0.1 * voltageMotor2);
            if (theta2 <= -80)
                theta2 = -80;
            else if (theta2 >= 90)
                theta2 = 90;

            //Update outputs
            o["Theta1"] = theta1;
            o["Theta2"] = theta2;
        }
    }
}
