using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace DivineJobs.Core
{
    /// <summary>
    /// Divine Job stage. Gives bonuses and advancing opportunities.
    /// </summary>
    public class DivineJobsLevelStage : IJobStageModifiers
    {
        /// <summary>
        /// Modifes stats with offsets.
        /// </summary>
        public List<StatModifier> statOffsets = new List<StatModifier>();
        /// <summary>
        /// Modifies capacities with either offsets or factors.
        /// </summary>
        public List<PawnCapacityModifier> capacityModifiers = new List<PawnCapacityModifier>();
        /// <summary>
        /// Denotes the maximum skill level that is now unlocked. Raises the soft cap.
        /// </summary>
        public List<SkillRequirement> skillMaxLevels = new List<SkillRequirement>();
        /// <summary>
        /// Denotes unlocked abilities.
        /// </summary>
        public List<DivineAbilityDef> abilities = new List<DivineAbilityDef>();
        /// <summary>
        /// Modifies the HealthScale with this.
        /// </summary>
        //public float healthScaleModifier = 1f;
        /// <summary>
        /// Modifies the BodyScale with this.
        /// </summary>
        //public float bodySizeModifier = 1f;

        public IEnumerable<StatModifier> StatOffsets => statOffsets;

        public IEnumerable<PawnCapacityModifier> CapacityModifiers => capacityModifiers;

        public IEnumerable<SkillRequirement> SkillMaxLevels => skillMaxLevels;
    }
}
