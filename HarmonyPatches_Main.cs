using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using RimWorld;
using System.Reflection;

namespace DivineJobs.Core
{
    /// <summary>
    /// Main class for managing patching through Harmony and other things.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class HarmonyPatches_Main
    {
        static HarmonyPatches_Main()
        {
            PatchDefs();
            ResolveJobs();
            PatchAssembly();
        }

        public static void ResolveJobs()
        {
            foreach(DivineJobDef jobDef in DefDatabase<DivineJobDef>.AllDefs)
            {
                //Resolve linked to and from jobs.
                IEnumerable<JobRequirement_Job> jobRequirements = jobDef.jobRequirements.Where(jobReq => jobReq is JobRequirement_Job)?.Select(jobReq => jobReq as JobRequirement_Job);
                if(jobRequirements != null)
                {
                    foreach(JobRequirement_Job jobReq in jobRequirements)
                    {
                        jobDef.linkedJobsFrom.Add(jobReq.jobDef);
                        jobReq.jobDef.linkedJobsTo.Add(jobDef);
                    }
                }
            }
        }

        public static void PatchDefs()
        {
            if(Prefs.DevMode)
            {
                Log.Message($"Patching DivineJobs to Humanlike ThingDefs with '{nameof(CompProperties_DivineJobs)}' and '{nameof(ITab_DivineJobs)}'.");
            }
            //Patch all compatible ThingDefs.
            //Requires: Can be Colonist
            foreach(ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if(thingDef?.race?.Humanlike ?? false)
                {
                    if (thingDef.comps == null)
                    {
                        thingDef.comps = new List<CompProperties>();
                    }
                    thingDef.comps.Add(new CompProperties_DivineJobs());

                    if(thingDef.inspectorTabs == null)
                    {
                        thingDef.inspectorTabs = new List<Type>();
                    }
                    //Insert next to Bio tab.
                    int bioTabPos = thingDef.inspectorTabs.IndexOf(typeof(ITab_Pawn_Character));
                    if (bioTabPos < 0)
                        bioTabPos = 0;
                    thingDef.inspectorTabs.Insert(bioTabPos, typeof(ITab_DivineJobs));
                    thingDef.inspectorTabsResolved.Insert(bioTabPos, InspectTabManager.GetSharedInstance(typeof(ITab_DivineJobs)));

                    if (Prefs.DevMode)
                    {
                        Log.Message($"=>{thingDef.defName} Patched");
                    }
                }
            }
        }

        public static void PatchAssembly()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("chjees.divinejobs.0.core");

            HarmonyPatches_Pawn.DoPatching(harmony);
            HarmonyPatches_Misc.DoPatching(harmony);

            harmony.PatchAll();
        }
    }
}
