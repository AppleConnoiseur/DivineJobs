using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace DivineJobs.Core
{
    public class AbilityTargeting_SingleTarget : AbilityTargetingWorker
    {
        public bool targetFriendly = true;
        public bool targetEnemy = true;
        public bool targetSelf = false;
        public bool targetPawn = true;
        public bool targetBuilding = false;
        public bool targetAnywhere = false;

        public override bool CanTarget(Pawn targeter, DivineJobsComp jobsComp, LocalTargetInfo target)
        {
            if(targetAnywhere)
            {
                return true;
            }

            if (targetPawn)
            {
                if(target.Thing is Pawn pawn)
                {
                    if(targetSelf && pawn == targeter)
                    {
                        return true;
                    }

                    if (pawn.HostileTo(targeter))
                    {
                        return targetEnemy;
                    }
                    else
                    {
                        return targetFriendly;
                    }
                }
            }

            if(targetBuilding)
            {
                if (target.Thing is Building building)
                {
                    if (building.HostileTo(targeter))
                    {
                        return targetEnemy;
                    }
                    else
                    {
                        return targetFriendly;
                    }
                }
            }

            return false;
        }

        public override LocalTargetInfo FindTargetForAI(Pawn targeter, DivineJobsComp jobsComp)
        {
            //Find the closest target. Priorotise targets it can see.
            return LocalTargetInfo.Invalid;
        }

        public override string GetComponentExplanation(AbilityData abilityData)
        {
            return "";
        }
    }
}
