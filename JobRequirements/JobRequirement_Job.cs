using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    /// <summary>
    /// Jobception! Also special in use when building the Job graph.
    /// </summary>
    public class JobRequirement_Job : JobRequirementWorker
    {
        public DivineJobDef jobDef;

        public override bool IsRequirementMet(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            return comp.TryGetJob(jobDef, out JobData data) && data.IsFullyLeveled;
        }

        public override string RequirementExplanation(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            if (IsRequirementMet(def, comp, pawn))
            {
                return "DivineJobs_JobRequirement_Job_Success".Translate(jobDef.LabelCap);
            }
            else
            {
                return "DivineJobs_JobRequirement_Job_Failed".Translate(jobDef.LabelCap);
            }
        }
    }
}
