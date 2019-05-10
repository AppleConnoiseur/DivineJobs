using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    public class AbilityData : IExposable
    {
        public Pawn owner;
        public DivineJobsComp jobsComp;

        public DivineAbilityDef def;

        public int cooldownTicksLeft;

        public AbilityData()
        {

        }

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref cooldownTicksLeft, "cooldownTicksLeft");
        }

        public virtual void PostMake()
        {

        }

        public virtual void Tick()
        {

        }

        public virtual void Notify_AcquireThisAbility()
        {

        }

        public virtual void RefreshAbility()
        {

        }

        public Gizmo MakeDefaultGizmo()
        {
            Command_Action gizmo = new Command_Action();
            
            return gizmo;
        }

        public virtual IEnumerable<Gizmo> MakeGizmos()
        {
            yield return MakeDefaultGizmo();
        }
    }
}
