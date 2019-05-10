using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using System.Reflection;
using UnityEngine;

namespace DivineJobs.Core
{
    public static class HarmonyPatches_Misc
    {
        public static void DoPatching(HarmonyInstance harmony)
        {
            //Patch StatWorkers
            {
                //GetValueUnfinalized
                Type type = typeof(StatWorker);
                MethodBase originalMethod = AccessTools.Method(type, "GetValueUnfinalized");
                HarmonyMethod method = new HarmonyMethod(typeof(HarmonyPatches_Misc), nameof(Patch_StatWorker_GetValueUnfinalized));
                harmony.Patch(originalMethod,
                    null, method);
            }
            {
                //GetExplanationUnfinalized
                Type type = typeof(StatWorker);
                MethodInfo method = type.GetMethod("GetExplanationUnfinalized");
                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches_Misc), nameof(Patch_StatWorker_GetExplanationUnfinalized));
                harmony.Patch(method, null, patchMethod);
            }
            //Patch SkillRecord
            {
                //Learn
                Type type = typeof(SkillRecord);
                MethodInfo method = type.GetMethod("Learn");
                HarmonyMethod patchMethodPrefix = new HarmonyMethod(typeof(HarmonyPatches_Misc), nameof(Patch_SkillRecord_Learn_Prefix));
                HarmonyMethod patchMethodSuffix = new HarmonyMethod(typeof(HarmonyPatches_Misc), nameof(Patch_SkillRecord_Learn_Suffix));
                harmony.Patch(method, patchMethodPrefix, patchMethodSuffix);
            }
            //Patch SkillUI
            {
                //DrawSkill
                Type type = typeof(SkillUI);
                MethodInfo method = type.GetMethod("DrawSkill", new Type[] { typeof(SkillRecord), typeof(Rect), typeof(SkillUI.SkillDrawMode), typeof(string) });
                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches_Misc), nameof(Patch_SkillUI_DrawSkill));
                harmony.Patch(method, null, patchMethod);
            }
        }

        public static void Patch_SkillUI_DrawSkill(SkillRecord skill, ref Rect holdingRect)
        {
            Pawn pawn = (Pawn)AccessTools.Field(typeof(SkillRecord), "pawn").GetValue(skill);
            if (pawn != null && pawn.GetJobsComp() is DivineJobsComp jobsComp)
            {
                float levelVerticalPosition = 0f;
                float maxLevel = 5f;
                if (jobsComp.SkillMaxLevels.FirstOrDefault(sr => sr.skill == skill.def) is SkillRequirement skillMaxLevel)
                {
                    maxLevel = skillMaxLevel.minLevel;
                }

                Rect rect = new Rect(holdingRect.x + 6f, holdingRect.y, (float)AccessTools.Field(typeof(SkillUI), "levelLabelWidth").GetValue(null) + 6f, holdingRect.height);
                Rect position = new Rect(rect.xMax, 0f, 24f, 24f);
                Rect rect2 = new Rect(position.xMax, holdingRect.y, holdingRect.width - position.xMax, holdingRect.height);
                levelVerticalPosition = rect2.width * (maxLevel / SkillRecord.MaxLevel);

                Rect skillBlip = rect2;
                skillBlip.width = 3f;
                skillBlip.x += levelVerticalPosition - 1f;
                Widgets.DrawBoxSolid(skillBlip, DivineJobsUI.SkillBlipColor);
                TooltipHandler.TipRegion(holdingRect, "DivineJobs_MaxLevel".Translate(maxLevel));
            }
        }

        public static bool Patch_SkillRecord_Learn_Prefix(SkillRecord __instance, Pawn ___pawn, ref float xp, bool direct)
        {
            if (xp > 0f && ___pawn != null && ___pawn.GetJobsComp() is DivineJobsComp jobsComp)
            {
                float learningModifer = 1f;
                if (jobsComp.SkillMaxLevels.FirstOrDefault(sr => sr.skill == __instance.def) is SkillRequirement skillMaxLevel)
                {
                    if (__instance.Level > skillMaxLevel.minLevel)
                    {
                        learningModifer = 0.25f;
                    }
                }
                else
                {
                    if (__instance.Level > 5)
                    {
                        learningModifer = 0.25f;
                    }
                }

                xp *= learningModifer;
            }

            return true;
        }

        public static void Patch_SkillRecord_Learn_Suffix(SkillRecord __instance, Pawn ___pawn, float xp, bool direct)
        {
            if (xp > 0f && ___pawn != null && ___pawn.GetJobsComp() is DivineJobsComp jobsComp)
            {
                jobsComp.AddExperience(xp, ExperienceSource.General);
            }
        }

        public static void Patch_StatWorker_GetValueUnfinalized(StatWorker __instance, ref float __result, ref StatRequest req, bool applyPostProcess, StatDef ___stat)
        {
            Pawn pawn = req.Thing as Pawn;
            if (pawn != null && pawn.GetJobsComp() is DivineJobsComp jobsComp)
            {
                if (jobsComp.StatOffsets.FirstOrDefault(mod => mod.stat == ___stat) is StatModifier statModifer)
                {
                    __result += statModifer.value;
                }
            }
        }

        public static void Patch_StatWorker_GetExplanationUnfinalized(StatWorker __instance, ref string __result, ref StatRequest req, ToStringNumberSense numberSense, StatDef ___stat)
        {
            Pawn pawn = req.Thing as Pawn;
            if (pawn != null && pawn.GetJobsComp() is DivineJobsComp jobsComp)
            {
                StringBuilder builder = new StringBuilder(__result);

                if (jobsComp.StatOffsets.FirstOrDefault(mod => mod.stat == ___stat) is StatModifier statModifer)
                {
                    builder.AppendLine();
                    builder.AppendLine();
                    builder.AppendLine("DivineJobs_Stats_JobTitle".Translate());

                    builder.AppendLine("    " + "DivineJobs_Stats_FromJobs".Translate() + ": " + statModifer.value.ToStringByStyle(statModifer.stat.ToStringStyleUnfinalized, ToStringNumberSense.Offset));
                    __result = builder.ToString().TrimEndNewlines();
                }
            }
        }
    }
}
