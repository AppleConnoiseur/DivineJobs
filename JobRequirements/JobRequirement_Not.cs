using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    public class JobRequirement_Not : JobRequirementWorker
    {
        public JobRequirementWorker requirement;

        public override bool IsRequirementMet(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            return !requirement.IsRequirementMet(def, comp, pawn);
        }

        public override string RequirementExplanation(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            return "DivineJobs_JobRequirement_Not".Translate(requirement.RequirementExplanation(def, comp, pawn));
        }
    }
}
