using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    public class JobRequirement_AddedParts : JobRequirementWorker
    {
        public TechLevel minTechLevel = TechLevel.Medieval;
        public int minAmount = 1;

        public int CountAddedParts(Pawn pawn)
        {
            int counted = 0;

            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if(hediff.def.spawnThingOnRemoved is ThingDef thingDef && thingDef.techLevel >= minTechLevel)
                {
                    counted++;
                }
            }

            return counted;
        }

        public override bool IsRequirementMet(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            return CountAddedParts(pawn) >= minAmount;
        }

        public override string RequirementExplanation(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            if (IsRequirementMet(def, comp, pawn))
            {
                return "DivineJobs_JobRequirement_AddedParts_Success".Translate(CountAddedParts(pawn), minAmount, minTechLevel.ToStringHuman().CapitalizeFirst());
            }
            else
            {
                return "DivineJobs_JobRequirement_AddedParts_Failed".Translate(CountAddedParts(pawn), minAmount, minTechLevel.ToStringHuman().CapitalizeFirst());
            }
        }
    }
}
