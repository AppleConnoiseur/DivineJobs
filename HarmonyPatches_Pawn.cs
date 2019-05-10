using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using System.Reflection;
using Harmony;

namespace DivineJobs.Core
{
    public static class HarmonyPatches_Pawn
    {
        public static void DoPatching(HarmonyInstance harmony)
        {
            /*{
                //HealthScale
                Type type = typeof(Pawn);
                MethodBase originalMethod = AccessTools.Property(type, "HealthScale").GetGetMethod();
                HarmonyMethod method = new HarmonyMethod(typeof(HarmonyPatches_Pawn), nameof(Patch_Pawn_HealthScale));
                harmony.Patch(originalMethod,
                    null, method);
            }
            {
                //BodySize
                Type type = typeof(Pawn);
                MethodBase originalMethod = AccessTools.Property(type, "BodySize").GetGetMethod();
                HarmonyMethod method = new HarmonyMethod(typeof(HarmonyPatches_Pawn), nameof(Patch_Pawn_BodySize));
                harmony.Patch(originalMethod,
                    null, method);
            }*/
        }

        /*public static void Patch_Pawn_HealthScale(ref Pawn __instance, ref float __result)
        {
            if(__instance.GetJobsComp() is DivineJobsComp jobsComp)
            {
                __result *= jobsComp.HealthScaleModifier;
            }
        }

        public static void Patch_Pawn_BodySize(ref Pawn __instance, ref float __result)
        {
            if (__instance.GetJobsComp() is DivineJobsComp jobsComp)
            {
                __result *= jobsComp.BodySizeModifier;
            }
        }*/
    }
}
