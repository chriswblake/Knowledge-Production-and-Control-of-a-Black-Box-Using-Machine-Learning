using System;
using BlackBoxModeling;
using BlackBoxModeling.Samples;

namespace TestingConsole
{
    class Program
    {
        static TrigFunctions sf = new TrigFunctions();

        static void Main(string[] args)
        {
            while(true)
            {
                Console.WriteLine("Press enter to display the black box value at time t.");

                //Get X from user
                Console.Write("Please enter a value for 'x': ");
                try
                { 
                    double x = Convert.ToDouble(Console.ReadLine());
                    //Set X
                    sf.Input["x"] = x;
                }
                catch
                {}

                //Read value of x and display
                double t = sf.TimeCurrent_ms;
                double y = Convert.ToDouble(sf.Output["y"]);
                Console.WriteLine("t:{0} \t y={1}", t, y);
            }
        }
    }
}
