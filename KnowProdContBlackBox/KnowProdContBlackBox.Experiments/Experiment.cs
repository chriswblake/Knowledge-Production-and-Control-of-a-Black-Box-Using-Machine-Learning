using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KnowProdContBlackBox.Experiments
{
    public class Experiment
    {
        protected string ResultsDir = System.IO.Path.GetFullPath(@"..\..\..\Results\");

        //Constructor
        public Experiment()
        {
            //Determine name of folder
            string className = this.GetType().Name;
            className = className.Replace("Experiments", "");
            className = className.Replace("Experiment", "");

            //Update result dir
            ResultsDir = Path.Combine(ResultsDir, className);

            //Create folder if missing
            if (!Directory.Exists(ResultsDir))
                Directory.CreateDirectory(ResultsDir);
        }
    }
}
