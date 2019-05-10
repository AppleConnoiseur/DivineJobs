using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace DivineJobs.Core
{
    public class JobRequirement_Race : JobRequirementWorker
    {
        public ThingDef raceDef;

        public override bool IsRequirementMet(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            return pawn.def == raceDef;
        }

        public override string RequirementExplanation(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            if (IsRequirementMet(def, comp, pawn))
            {
                return "DivineJobs_JobRequirement_Race_Success".Translate(raceDef.LabelCap);
            }
            else
            {
                return "DivineJobs_JobRequirement_Race_Failed".Translate(raceDef.LabelCap);
            }
        }
    }
}
