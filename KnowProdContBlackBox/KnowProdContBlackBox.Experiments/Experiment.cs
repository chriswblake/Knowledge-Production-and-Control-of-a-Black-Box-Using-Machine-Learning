using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace KnowProdContBlackBox.Experiments
{
    public class Experiment
    {
        //Properties
        protected string ResultsDir
        {
            get
            {
                //Get path components
                string basePath = Path.GetFullPath(@"..\..\..\Results\");
                string className = GetClassName();
                string methodName = GetCurrentMethodName(2);

                //Combine
                string result = Path.Combine(basePath, className, methodName);

                //Create folder if missing
                if (!Directory.Exists(result))
                    Directory.CreateDirectory(result);

                return result;
            }
        }
        //protected string ResultsDir2 = System.IO.Path.GetFullPath(@"..\..\..\Results\");

        //Methods
        private string GetClassName()
        {
            string className = this.GetType().Name;
            className = className.Replace("Experiments", "");
            className = className.Replace("Experiment", "");
            return className;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private string GetCurrentMethodName(int stepsBack)
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1 + stepsBack);

            return sf.GetMethod().Name;
        }
    }
}
