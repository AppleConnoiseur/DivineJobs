using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace DivineJobs.Core
{
    public class JobRequirement_Trait : JobRequirementWorker
    {
        public TraitDef trait;
        public int minDegree = 0;
        public int maxDegree = 0;

        public override bool IsRequirementMet(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            return pawn.story.traits.HasTrait(trait) && pawn.story.traits.GetTrait(trait) is Trait traitInstance && traitInstance.Degree >= minDegree && traitInstance.Degree <= maxDegree;
        }

        public string TraitsBetweenDegrees()
        {
            StringBuilder builder = new StringBuilder();
            for(int i = minDegree; i < maxDegree; i++)
            {
                if(i == maxDegree)
                {
                    builder.Append($"{trait.DataAtDegree(i).label}");
                }
                else
                {
                    builder.Append($"{trait.DataAtDegree(i).label} / ");
                }
            }
            return builder.ToString();
        }

        public override string RequirementExplanation(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            if (IsRequirementMet(def, comp, pawn))
            {
                return "DivineJobs_JobRequirement_Trait_Success".Translate(TraitsBetweenDegrees());
            }
            else
            {
                return "DivineJobs_JobRequirement_Trait_Failed".Translate(TraitsBetweenDegrees());
            }
        }
    }
}
