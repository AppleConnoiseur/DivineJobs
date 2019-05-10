using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    /// <summary>
    /// Works out whether a requirement of a Job is fulfilled or not.
    /// </summary>
    public abstract class JobRequirementWorker
    {
        public abstract bool IsRequirementMet(DivineJobDef def, DivineJobsComp comp, Pawn pawn);
        public abstract string RequirementExplanation(DivineJobDef def, DivineJobsComp comp, Pawn pawn);
    }
}
