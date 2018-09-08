using System;
using System.Collections.Generic;
using System.Text;

namespace BlackBoxModeling.Samples
{
    public class RoboticArm : BlackBox
    {
        //Fields
        const double LENGTH1 = 300; //mm
        double theta1 = 225; //degrees (180 to 315)
        double length2 = 150; //mm (100 to 300)
        double theta3 = 225; //degrees (90 to 180)
        const double LENGTH3 = 50; //mm
        double x = 0;
        double y = 0;

        //Constructors
        public RoboticArm()
        {
            //Set Name
            this.Name = "Robotic Arm";

            //Define inputs
            AddInput("Motor1");
            AddInput("Motor2");
            AddInput("Motor3");

            //Define outputs
            AddOutput("Theta1");
            AddOutput("Length2");
            AddOutput("Theta3");
            AddOutput("x");
            AddOutput("y");

            //Specify responsiveness
            this.TimeInterval_ms = 250; //i.e. The model needs 0.250 seconds to convert the input into a new output.
        }
        public override void Run()
        {
            var i = Input;
            var o = Output;

            double voltageMotor1 = Convert.ToDouble(i["Motor1"]);
            double voltageMotor2 = Convert.ToDouble(i["Motor2"]);
            double voltageMotor3 = Convert.ToDouble(i["Motor3"]);

            //Update theta1
            theta1 = theta1 + (0.1 * voltageMotor1);
            if (theta1 <= 180)
                theta1 = 180;
            else if (theta1 >= 315)
                theta1 = 315;

            //Update theta2
            length2 = length2 + (0.1 * voltageMotor2);
            if (length2 <= 100)
                length2 = 100;
            else if (length2 >= 300)
                length2 = 300;

            //Update theta3
            theta3 = theta3 + (0.1 * voltageMotor1);
            if (theta3 <= 180)
                theta3 = 180;
            else if (theta3 >= 315)
                theta3 = 315;

            //Update x and y coordinates of end factor
            double theta1abs = 180 + theta1;
            double theta3abs = theta1abs - 180 + theta3;
            x = LENGTH1
                       + Math.Cos(Radians(theta1abs)) * length2
                       + Math.Cos(Radians(theta3abs)) * LENGTH3;
            y = 0
                        + Math.Sin(Radians(theta1abs)) * length2
                        + Math.Sin(Radians(theta3abs)) * LENGTH3;

            //Update outputs
            o["Theta1"] = theta1;
            o["Length2"] = length2;
            o["Theta3"] = theta3;
            o["x"] = x;
            o["y"] = y;
        }

        //Methods
        double Radians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
