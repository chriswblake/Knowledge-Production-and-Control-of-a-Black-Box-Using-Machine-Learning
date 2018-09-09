using System;
using System.Collections.Generic;
using System.Text;

namespace BlackBoxModeling.Samples
{
    public class RoboticArm : BlackBox
    {
        //Fields
        const double LENGTH1 = 300;     //mm
        const double MOTORCONST1 = 50;  //deg/(sec*V)
        double theta1 = 135;            //deg, range=45 to 180
        const double MOTORCONST2 = 50;  //mm/(sec*V)
        double length2 = 150;           //mm, range=100 to 300
        const double MOTORCONST3 = 50;  //deg/(sec*V)
        double theta3 = 135;            //deg, range=90 to 270
        const double LENGTH3 = 50;      //mm
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
            theta1 = theta1 + (MOTORCONST1 * this.TimeInterval_ms * voltageMotor1 );
            if (theta1 <= 45)
                theta1 = 45;
            else if (theta1 >= 180)
                theta1 = 180;

            //Update length 2
            length2 = length2 + (MOTORCONST2 * this.TimeInterval_ms * voltageMotor2);
            if (length2 <= 100)
                length2 = 100;
            else if (length2 >= 300)
                length2 = 300;

            //Update theta3
            theta3 = theta3 + (MOTORCONST3 * this.TimeInterval_ms * voltageMotor1);
            if (theta3 <= 90)
                theta3 = 90;
            else if (theta3 >=270)
                theta3 = 270;

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
