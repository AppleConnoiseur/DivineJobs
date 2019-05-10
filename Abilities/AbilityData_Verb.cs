using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    public class AbilityData_Verb : AbilityData, IVerbOwner
    {
        public VerbTracker verbTracker;

        public AbilityData_Verb()
        {
            verbTracker = new VerbTracker(this);
        }

        public VerbTracker VerbTracker => verbTracker;

        public List<VerbProperties> VerbProperties => def.Verbs;

        public List<Tool> Tools => null; //owner.def.tools

        public ImplementOwnerTypeDef ImplementOwnerTypeDef => ImplementOwnerTypeDefOf.Weapon;

        public Thing ConstantCaster => null;

        public Verb PrimaryVerb
        {
            get
            {
                return verbTracker.PrimaryVerb;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref verbTracker, "verbToUse", new object[] { this });
        }

        public override void Tick()
        {
            base.Tick();
            verbTracker.VerbsTick();
        }

        public string UniqueVerbOwnerID()
        {
            return $"AbilityData_Verb_{owner.ThingID}";
        }

        public bool VerbsStillUsableBy(Pawn p)
        {
            return jobsComp.TryGetAbility(def, out AbilityData data);
        }
    }
}
