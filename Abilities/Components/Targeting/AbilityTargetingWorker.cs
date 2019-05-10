using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    /// <summary>
    /// Helps both the player and AI find valid targets.
    /// </summary>
    public abstract class AbilityTargetingWorker : IAbilityComponentExplanation
    {
        /// <summary>
        /// Explains in human readable terms what the component does.
        /// </summary>
        /// <returns>Explanation part.</returns>
        public abstract string GetComponentExplanation(AbilityData abilityData);

        public abstract bool CanTarget(Pawn targeter, DivineJobsComp jobsComp, LocalTargetInfo target);

        public abstract LocalTargetInfo FindTargetForAI(Pawn targeter, DivineJobsComp jobsComp);
    }
}
