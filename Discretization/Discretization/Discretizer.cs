using System;
using System.Collections.Generic;
using System.Text;

namespace Discretization
{
    public class Discretizer
    {
        //Properties
        public List<Bin> Bins = new List<Bin>();

        //Constuctors
        public Discretizer()
        {
            //Create first bin to accept all values
            Bins.Add(new Bin());
        }

        //Methods
        public Bin GetBin(double value)
        {
            //Find appropriate bin
            Bin theBin = Bins.Find(p => value >= p.Low && value < p.High); //inclusive low. exclusive high

            //Add this value to that bin
            theBin.AddValue(value);

            //Split the bin if needed
            if (theBin.ShouldSplit())
                SplitBin(theBin);

            //Return that bin as the selected bin
            return theBin;
        }
        private void SplitBin(Bin theBin)
        {
            //Remove existing bin
            Bins.Remove(theBin);
            
            //Create new bins
            Bins.Add(new Bin(theBin.Low, theBin.Average));
            Bins.Add(new Bin(theBin.Average, theBin.High));   
        }

        //Debug
        public override string ToString()
        {
            return string.Format("Bins={0}", Bins.Count);
        }
    }
}
