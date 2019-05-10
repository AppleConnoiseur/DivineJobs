using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace DivineJobs.Core
{
    public class JobRequirement_Technology : JobRequirementWorker
    {
        public ResearchProjectDef research;

        public override bool IsRequirementMet(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            return research.IsFinished;
        }

        public override string RequirementExplanation(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            if (IsRequirementMet(def, comp, pawn))
            {
                return "DivineJobs_JobRequirement_Technology_Success".Translate(research.LabelCap);
            }
            else
            {
                return "DivineJobs_JobRequirement_Technology_Failed".Translate(research.LabelCap);
            }
        }
    }
}
