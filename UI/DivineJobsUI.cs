using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DivineJobs.Core
{
    public static class DivineJobsUI
    {
        public static float RowHeight = 24f;
        public static Color AlternateColor = new Color(0.31f, 0.32f, 0.33f);
        public static Color SkillBlipColor = new Color(1f, 1f, 0f, 0.5f);
        public static Color highlightColor;
        public static Color highlightLineColor;

        static DivineJobsUI()
        {
            ColorInt colorHighlight = new ColorInt(51, 205, 217, 255);
            highlightColor = colorHighlight.ToColor;

            ColorInt colorLineHighlight = new ColorInt(255, 217, 83, 255);
            highlightLineColor = colorLineHighlight.ToColor;
        }

        public static string GenerateJobRequirementReport(DivineJobDef def, DivineJobsComp comp, Pawn pawn)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("DivineJobs_JobRequirementReport".Translate() + ":");
            if(def.jobRequirements.Count > 0)
            {
                foreach (JobRequirementWorker req in def.jobRequirements)
                {
                    bool isMet = req.IsRequirementMet(def, comp, pawn);
                    builder.AppendLine($"    {isMet.BoolIntoSymbol()} {req.RequirementExplanation(def, comp, pawn)}");
                }
            }
            else
            {
                builder.AppendLine($"    {"DivineJobs_JobRequirementReport_None".Translate()}");
            }

            return builder.ToString();
        }

        public static float CalculateHeightOfStageModifier(IJobStageModifiers stage)
        {
            float finalHeight = 0f;
            /*if(stage.bodySizeModifier != 1.0f)
            {
                finalHeight += RowHeight;
            }
            if (stage.healthScaleModifier != 1.0f)
            {
                finalHeight += RowHeight;
            }*/
            finalHeight += RowHeight * stage.StatOffsets.Count();
            finalHeight += RowHeight * stage.CapacityModifiers.Count();
            finalHeight += RowHeight * stage.SkillMaxLevels.Count();

            return finalHeight;
        }

        public static void FillStageModifier(Rect inRect, IJobStageModifiers stage, bool isSelected, Pawn pawn = null)
        {
            const float middle = 0.8f;

            bool alternateField = false;
            Rect rowRect = inRect;
            rowRect.height = RowHeight;
            /*if (stage.healthScaleModifier != 1.0f)
            {
                FillSimpleTableRow(ref alternateField, rowRect, "DivineJobs_Stage_HealthModifier".Translate(), stage.healthScaleModifier.ToStringPercent(), middle);
                rowRect.y += RowHeight;
            }
            if (stage.bodySizeModifier != 1.0f)
            {
                FillSimpleTableRow(ref alternateField, rowRect, "DivineJobs_Stage_BodyModifier".Translate(), stage.bodySizeModifier.ToStringPercent(), middle);
                rowRect.y += RowHeight;
            }*/
            if (stage.SkillMaxLevels.Count() > 0)
            {
                foreach(SkillRequirement skillMaxLevel in stage.SkillMaxLevels)
                {
                    FillSimpleTableRow(ref alternateField, rowRect, "DivineJobs_Stage_MaxLevel".Translate(skillMaxLevel.skill.LabelCap), $"{skillMaxLevel.minLevel}", middle);
                    rowRect.y += RowHeight;
                }
            }
            if (stage.StatOffsets.Count() > 0)
            {
                foreach(StatModifier statModifier in stage.StatOffsets)
                {
                    FillSimpleTableRow(ref alternateField, rowRect, statModifier.stat.LabelCap, statModifier.ValueToStringAsOffset, middle);
                    rowRect.y += RowHeight;
                }
            }
            if (stage.CapacityModifiers.Count() > 0)
            {
                foreach (PawnCapacityModifier capacityModifier in stage.CapacityModifiers)
                {
                    FillSimpleTableRow(ref alternateField, rowRect, pawn==null ? capacityModifier.capacity.LabelCap : capacityModifier.capacity.GetLabelFor(pawn), capacityModifier.offset.ToStringPercent(), middle);
                    rowRect.y += RowHeight;
                }
            }
        }

        public static void FillSimpleTableRow(ref bool fieldAlternator, Rect rect, string left, string right, float middle = 0.5f)
        {
            float middlePosition = rect.width * middle;
            Rect leftRect = rect;
            Rect rightRect = rect;
            leftRect.width = middlePosition;
            rightRect.width -= leftRect.width;
            rightRect.x += leftRect.width;
            leftRect = leftRect.ContractedBy(2f);
            rightRect = rightRect.ContractedBy(2f);

            //Draw sides
            if(fieldAlternator)
            {
                Widgets.DrawBoxSolid(leftRect, AlternateColor);
                Widgets.DrawBoxSolid(rightRect, AlternateColor);
            }

            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Small;

            Widgets.Label(leftRect, left);
            Widgets.Label(rightRect, right);

            Text.Anchor = TextAnchor.UpperLeft;

            //Flip
            fieldAlternator = !fieldAlternator;
        }

        public static void CollapsibleSectionHeader(Rect rect, SectionRect section, string text)
        {
            if (Widgets.ButtonText(rect, text))
            {
                section.isCollapsed = !section.isCollapsed;
            }
        }

        public static Rect CenterOfRectForRect(this Rect rect, Rect forRect)
        {
            Rect result = rect;
            result.x = rect.x + (rect.width * 0.5f) - (forRect.width * 0.5f);
            result.y = rect.y + (rect.height * 0.5f) - (forRect.height * 0.5f);

            return result;
        }

        public static Rect CenterOfRectForRect(this Rect rect, Vector2 size)
        {
            Rect result = rect;
            result.x = rect.x + (rect.width * 0.5f) - (size.x * 0.5f);
            result.y = rect.y + (rect.height * 0.5f) - (size.y * 0.5f);

            return result;
        }
    }
}
