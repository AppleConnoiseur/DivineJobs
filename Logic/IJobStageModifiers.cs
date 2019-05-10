using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    public interface IJobStageModifiers
    {
        IEnumerable<StatModifier> StatOffsets { get; }
        IEnumerable<PawnCapacityModifier> CapacityModifiers { get; }
        IEnumerable<SkillRequirement> SkillMaxLevels { get; }
    }
}
