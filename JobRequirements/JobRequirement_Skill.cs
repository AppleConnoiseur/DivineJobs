using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    public class JobRequirement_Skill : JobRequirementWorker
    {
        public SkillDef skill;
        public int minimumSkillRequired = 0;

        public override bool IsRequirementMet(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            return pawn.skills.GetSkill(skill).Level >= minimumSkillRequired;
        }

        public override string RequirementExplanation(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            int skillLevel = pawn.skills.GetSkill(skill).Level;

            if (IsRequirementMet(def, comp, pawn))
            {
                return "DivineJobs_JobRequirement_Skill_Success".Translate(skill.LabelCap, skillLevel, minimumSkillRequired);
            }
            else
            {
                return "DivineJobs_JobRequirement_Skill_Failed".Translate(skill.LabelCap, skillLevel, minimumSkillRequired);
            }
        }
    }
}
