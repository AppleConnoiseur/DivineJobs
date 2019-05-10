using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    public class JobRequirement_Hediff : JobRequirementWorker
    {
        public HediffDef hediffDef;

        public override bool IsRequirementMet(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            return pawn.health.hediffSet.HasHediff(hediffDef);
        }

        public override string RequirementExplanation(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            if (IsRequirementMet(def, comp, pawn))
            {
                return "DivineJobs_JobRequirement_Hediff_Success".Translate(hediffDef.LabelCap);
            }
            else
            {
                return "DivineJobs_JobRequirement_Hediff_Failed".Translate(hediffDef.LabelCap);
            }
        }
    }
}
