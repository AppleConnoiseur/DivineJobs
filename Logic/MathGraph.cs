using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineJobs.Core
{
    /// <summary>
    /// Represents a mathematical two dimensional graph. Mainly used for converting a inputted X value into Y out value.
    /// </summary>
    public abstract class MathGraph
    {
        /// <summary>
        /// Converts the incoming X value to the appropiate Y value.
        /// </summary>
        /// <param name="inValue">Incoming X value.</param>
        /// <returns>Outcoming Y value.</returns>
        public abstract double Convert(double inValue);
    }
}
