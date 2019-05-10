using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    public class JobRequirement_JobWithTag : JobRequirementWorker
    {
        public DivineJobTagDef tag;
        public bool fullyLeveled = true;

        public JobData TryGetTaggedJob(DivineJobsComp comp)
        {
            JobData result;
            comp.TryGetJob(job => job.def.tags.Contains(tag), out result);
            return result;
        }

        public override bool IsRequirementMet(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            JobData data = TryGetTaggedJob(comp);
            if(data == null)
            {
                return false;
            }

            if (fullyLeveled)
            {
                return data.IsFullyLeveled;
            }
            else
            {
                return true;
            }
        }

        public override string RequirementExplanation(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            if (IsRequirementMet(def, comp, pawn))
            {
                JobData taggedJob = TryGetTaggedJob(comp);
                if (fullyLeveled)
                {
                    return "DivineJobs_JobRequirement_JobWithTag_Success".Translate(tag.LabelCap, taggedJob.def.LabelCap);
                }
                else
                {
                    return "DivineJobs_JobRequirement_JobWithTag_NotLeveled_Success".Translate(tag.LabelCap, taggedJob.def.LabelCap);
                }
            }
            else
            {
                if (fullyLeveled)
                {
                    return "DivineJobs_JobRequirement_JobWithTag_Failed".Translate(tag.LabelCap);
                }
                else
                {
                    return "DivineJobs_JobRequirement_JobWithTag_NotLeveled_Failed".Translate(tag.LabelCap);
                }
            }
        }
    }
}
