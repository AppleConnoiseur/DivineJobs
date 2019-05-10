using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace DivineJobs.Core
{
    public class AbilityEffect_CastVerbOnce : AbilityEffectWorker
    {
        public override void RefreshAbilityData(AbilityData abilityData)
        {
            
        }

        public override void ApplyEffect(AbilityData abilityData, Pawn effectApplier, LocalTargetInfo target)
        {
            AbilityData_Verb data = abilityData as AbilityData_Verb;
            effectApplier.TryStartAttack(target, data.PrimaryVerb, abilityData.def.isViolent, abilityData.def.canHitNonTargetPawns);
        }

        public override string GetComponentExplanation(AbilityData abilityData)
        {
            return "";
        }
    }
}
