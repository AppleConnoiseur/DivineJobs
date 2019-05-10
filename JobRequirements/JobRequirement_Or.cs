using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace DivineJobs.Core
{
    public class JobRequirement_Or : JobRequirementWorker
    {
        public JobRequirementWorker first;
        public JobRequirementWorker second;

        public override bool IsRequirementMet(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            return first.IsRequirementMet(def, comp, pawn) || second.IsRequirementMet(def, comp, pawn);
        }

        public override string RequirementExplanation(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            return "DivineJobs_JobRequirement_Or".Translate(first.RequirementExplanation(def, comp, pawn), second.RequirementExplanation(def, comp, pawn));
        }
    }
}
