using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineJobs.Core
{
    /// <summary>
    /// A exponentional equation: f(x) = baseValue * (exponentional^x)
    /// </summary>
    public class ExponentialGraph : MathGraph
    {
        double baseValue = 100d;

        double exponentional = 1.5d;

        public ExponentialGraph()
        {

        }

        public ExponentialGraph(double baseValue, double exponentional = 1.5d)
        {
            this.baseValue = baseValue;
            this.exponentional = exponentional;
        }

        public override double Convert(double inValue)
        {
            return baseValue * Math.Pow(exponentional, inValue);
        }
    }
}
