using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    public class JobRequirement_Record : JobRequirementWorker
    {
        public RecordDef record;
        public float minimumRequiredAmount;

        public override bool IsRequirementMet(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            return pawn.records.GetValue(record) >= minimumRequiredAmount;
        }

        public override string RequirementExplanation(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            string requiredAmount = "";
            if(record.type == RecordType.Int)
            {
                requiredAmount = pawn.records.GetAsInt(record).ToString();
            }
            else if (record.type == RecordType.Float)
            {
                requiredAmount = pawn.records.GetValue(record).ToString();
            }

            if (IsRequirementMet(def, comp, pawn))
            {
                return "DivineJobs_JobRequirement_Record_Success".Translate(record.LabelCap, minimumRequiredAmount, requiredAmount);
            }
            else
            {
                return "DivineJobs_JobRequirement_Record_Failed".Translate(record.LabelCap, minimumRequiredAmount, requiredAmount);
            }
        }
    }
}
