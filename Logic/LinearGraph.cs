using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineJobs.Core
{
    /// <summary>
    /// A linear equation: f(x) = stepValue * x
    /// </summary>
    public class LinearGraph : MathGraph
    {
        public double stepValue = 100d;

        public LinearGraph()
        {

        }

        public LinearGraph(double stepValue)
        {
            this.stepValue = stepValue;
        }

        public override double Convert(double inValue)
        {
            return stepValue * inValue;
        }
    }
}
