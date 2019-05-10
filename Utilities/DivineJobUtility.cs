using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    public static class DivineJobUtility
    {
        public static JobData MakeJobInstance(DivineJobDef def, Pawn owner = null, DivineJobsComp comp = null)
        {
            JobData instance = (JobData)Activator.CreateInstance(def.jobClass);
            instance.def = def;
            instance.owner = owner;
            instance.jobsComp = comp;
            return instance;
        }

        public static JobResource MakeResourceInstance(DivineJobResourceDef def, Pawn owner = null, DivineJobsComp comp = null)
        {
            JobResource instance = (JobResource)Activator.CreateInstance(def.resourceClass);
            instance.def = def;
            instance.owner = owner;
            instance.jobsComp = comp;
            instance.PostMake();
            return instance;
        }

        public static AbilityData MakeAbilityInstance(DivineAbilityDef def, Pawn owner = null, DivineJobsComp comp = null)
        {
            AbilityData instance = (AbilityData)Activator.CreateInstance(def.abilityClass);
            instance.def = def;
            instance.owner = owner;
            instance.jobsComp = comp;
            instance.PostMake();
            return instance;
        }

        public static DivineJobsComp GetJobsComp(this Pawn target)
        {
            return target.TryGetComp<DivineJobsComp>();
        }

        public static JobData GetDivineJob(this Pawn target, DivineJobDef def)
        {
            if(target.GetJobsComp() is DivineJobsComp comp)
            {
                return comp.jobs.FirstOrDefault(job => job.def == def);
            }

            return null;
        }

        public static JobResource GetDivineResource(this Pawn target, DivineJobResourceDef def)
        {
            if(target.GetJobsComp() is DivineJobsComp comp)
            {
                return comp.resources.FirstOrDefault(resource => resource.def == def);
            }

            return null;
        }

        public static AbilityData GetDivineAbility(this Pawn target, DivineAbilityDef def)
        {
            if (target.GetJobsComp() is DivineJobsComp comp)
            {
                return comp.abilities.FirstOrDefault(ability => ability.def == def);
            }

            return null;
        }

        public static bool CanAdoptJob(this Pawn pawn, DivineJobDef def)
        {
            DivineJobsComp comp = pawn.GetJobsComp();
            if(comp == null)
            {
                return false;
            }

            if(comp.jobs.Any(job => job.def == def))
            {
                return false;
            }

            if ((def.jobType == JobType.Normal && comp.activeJob != null && !comp.activeJob.IsFullyLeveled) ||
                (def.jobType == JobType.Race && comp.activeRaceJob != null && !comp.activeRaceJob.IsFullyLeveled))
            {
                return false;
            }

            foreach (JobRequirementWorker req in def.jobRequirements)
            {
                if(!req.IsRequirementMet(def, comp, pawn))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool TryStartAttack(this Pawn pawn, LocalTargetInfo targ, Verb forcedVerb, bool isViolent = true, bool canHitNonTargetPawns = true)
        {
            if (pawn.stances.FullBodyBusy)
            {
                return false;
            }
            if (pawn.story != null)
            {
                if(isViolent && pawn.story.WorkTagIsDisabled(WorkTags.Violent))
                {
                    return false;
                }
            }

            return forcedVerb.TryStartCastOn(targ, false, canHitNonTargetPawns);
        }
    }
}
