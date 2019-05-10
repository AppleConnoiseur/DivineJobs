using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    /// <summary>
    /// Defines a resource type for use within Jobs.
    /// </summary>
    public class DivineJobResourceDef : Def
    {
        public Type resourceClass = typeof(JobResource);
        public bool startWith = false;
        public int order = 0;

        //Numerical properties
        public float startingValue = 100f;
        public float startingMaxValue = 100f;
        public float baseValueRegeneration = 1f / GenTicks.TicksPerRealSecond;
    }
}
