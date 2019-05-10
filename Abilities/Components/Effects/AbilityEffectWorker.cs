using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    /// <summary>
    /// Applies effects onto the target.
    /// </summary>
    public abstract class AbilityEffectWorker : IAbilityComponentExplanation
    {
        /// <summary>
        /// Explains in human readable terms what the component does.
        /// </summary>
        /// <returns>Explanation part.</returns>
        public abstract string GetComponentExplanation(AbilityData abilityData);

        public abstract void ApplyEffect(AbilityData abilityData, Pawn effectApplier, LocalTargetInfo target);

        public virtual void RefreshAbilityData(AbilityData abilityData)
        {

        }
    }
}
