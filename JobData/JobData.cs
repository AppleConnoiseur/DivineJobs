using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace DivineJobs.Core
{
    /// <summary>
    /// Contains data needed in order to make a Job work.
    /// </summary>
    public class JobData : IExposable
    {
        public static Color highlightColor;
        public static Color highlightLineColor;
        public static Vector2 scrollPanePosition = new Vector2();
        public static List<SectionRect> jobSections = null;
        public static GraphView experienceGraph = null;

        public Pawn owner;
        public DivineJobsComp jobsComp;

        public DivineJobDef def;
        public int level;
        public double experience;

        public bool IsFullyLeveled
        {
            get
            {
                return level >= def.maxLevel - 1;
            }
        }

        public double ExperiencePercentToNextLevel
        {
            get
            {
                if(IsFullyLeveled)
                {
                    return 1d;
                }

                return (experience - def.ExperienceRequiredForLevel(level)) / (def.ExperienceRequiredForLevel(level + 1) - def.ExperienceRequiredForLevel(level));
            }
        }
        
        static JobData()
        {
            ColorInt colorHighlight = new ColorInt(51, 205, 217, 255);
            highlightColor = colorHighlight.ToColor;

            ColorInt colorLineHighlight = new ColorInt(255, 217, 83, 255);
            highlightLineColor = colorLineHighlight.ToColor;
        }

        public JobData()
        {

        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref level, "level");
            Scribe_Values.Look(ref experience, "experience");
        }

        public virtual DivineJobsLevelStage GetCurrentJobStage()
        {
            return def.levelStages[level];
        }

        /// <summary>
        /// Adds experience to this Job.
        /// </summary>
        /// <param name="amount">Amount of experience.</param>
        /// <param name="source">Experience source it came from.</param>
        public virtual void AddExperience(double amount, ExperienceSource source)
        {
            double experienceModifier = 1d;
            if(def.preferredExperienceSource != ExperienceSource.General && source != def.preferredExperienceSource)
            {
                experienceModifier = 0.5d;
            }

            experience += amount * experienceModifier;

            if(!IsFullyLeveled && level < def.maxLevel && experience >= def.ExperienceRequiredForLevel(level + 1))
            {
                level++;
                jobsComp.RefreshCachedModifiers();
                Notify_Levelup();
            }
        }

        /// <summary>
        /// Sets the level forcefully with the minimum required experience to get it.
        /// </summary>
        /// <param name="newLevel">The desired level to be set to.</param>
        public virtual void SetLevel(int newLevel)
        {
            level = newLevel;
            experience = def.ExperienceRequiredForLevel(level);
            jobsComp.RefreshCachedModifiers();
        }

        /// <summary>
        /// Job just got acquired notification.
        /// </summary>
        public virtual void Notify_AcquireThisJob()
        {

        }

        /// <summary>
        /// Level up notification.
        /// </summary>
        public virtual void Notify_Levelup()
        {
            //To-Do:
            //Play level up sound if possible.
            //Create sparkling motes at pawn.
        }

        /// <summary>
        /// Ticked when the Pawn ticks.
        /// </summary>
        public virtual void Tick()
        {

        }

        /// <summary>
        /// Draws the default Job GUI.
        /// </summary>
        /// <param name="jobRect">Area the Job got to work with.</param>
        public virtual void JobGUI(Rect jobRect)
        {
            DefaultJobGUI(jobRect);
        }

        public Rect DefaultJobGUISize(Rect jobRect)
        {
            Rect innerRect = jobRect;

            innerRect.height = 32f;
            foreach (SectionRect section in jobSections)
            {
                innerRect.height += section.Height;
            }

            return innerRect;
        }

        public void DefaultJobGUI(Rect jobRect)
        {
            //Setup sections
            SetupJobSections(jobRect);

            //Display general job information inside a scroll panel.
            Rect innerRect = DefaultJobGUISize(jobRect);

            Text.Font = GameFont.Small;
            Widgets.BeginScrollView(jobRect, ref scrollPanePosition, innerRect);

            Rect sectionRect = innerRect;
            sectionRect.height = 0f;

            foreach (SectionRect section in jobSections)
            {
                sectionRect.y += sectionRect.height;
                sectionRect.height = section.Height;
                section.renderAction(sectionRect);
            }

            Widgets.EndScrollView();
        }

        public void SetupJobSections(Rect jobRect)
        {
            if (jobSections == null)
            {
                jobSections = new List<SectionRect>();
                //
                //  Description
                //
                SectionRect descriptionSection = new SectionRect();
                descriptionSection.renderAction =
                    delegate (Rect rect)
                    {
                        //Head section.
                        Rect collapsibleRect = rect;
                        collapsibleRect.height = descriptionSection.collapsedHeight;

                        DivineJobsUI.CollapsibleSectionHeader(collapsibleRect, descriptionSection, "DivineJobs_Tab_Jobs_Description".Translate());

                        if (!descriptionSection.isCollapsed)
                        {
                            rect.height -= collapsibleRect.height;
                            rect.y += collapsibleRect.height;
                            Widgets.Label(rect, def.description);
                        }
                    };
                descriptionSection.heightGetter =
                    delegate ()
                    {
                        return DivineJobsUI.RowHeight + Text.CalcHeight(def.description, jobRect.width);
                    };
                jobSections.Add(descriptionSection);

                //
                //  Level Modifiers
                //
                SectionRect levelModifierSection = new SectionRect();
                levelModifierSection.renderAction =
                    delegate (Rect rect)
                    {
                        //Head section.
                        Rect collapsibleRect = rect;
                        collapsibleRect.height = levelModifierSection.collapsedHeight;

                        DivineJobsUI.CollapsibleSectionHeader(collapsibleRect, levelModifierSection, "DivineJobs_Tab_Jobs_LevelModifiers".Translate());

                        if (!levelModifierSection.isCollapsed)
                        {
                            rect.height -= collapsibleRect.height;
                            rect.y += collapsibleRect.height;

                            int stageInt = 0;
                            foreach (DivineJobsLevelStage stage in def.levelStages)
                            {
                                //Stage Level
                                rect.height = DivineJobsUI.RowHeight;
                                if(level == stageInt)
                                {
                                    Widgets.DrawBoxSolid(rect, Color.yellow);
                                }
                                else
                                {
                                    Widgets.DrawBoxSolid(rect, Color.cyan);
                                }
                                Color color = GUI.color;
                                GUI.color = Color.black;
                                Text.Anchor = TextAnchor.MiddleCenter;
                                Widgets.Label(rect, $"{stageInt + 1}");
                                Text.Anchor = TextAnchor.UpperLeft;
                                GUI.color = color;
                                rect.y += DivineJobsUI.RowHeight;

                                //Stage content
                                float stageHeight = DivineJobsUI.CalculateHeightOfStageModifier(stage);
                                rect.height = stageHeight;
                                DivineJobsUI.FillStageModifier(rect, stage, level == stageInt, owner);
                                rect.y += stageHeight;
                                stageInt++;
                            }
                        }
                    };
                levelModifierSection.heightGetter =
                    delegate ()
                    {
                        float result = DivineJobsUI.RowHeight;
                        foreach (DivineJobsLevelStage stage in def.levelStages)
                        {
                            result += DivineJobsUI.RowHeight + DivineJobsUI.CalculateHeightOfStageModifier(stage);
                        }
                        return result;
                    };
                jobSections.Add(levelModifierSection);

                //
                //  Experience graph -- experienceGraph
                //
                experienceGraph = new GraphView(def.ExperiencePerLevelRequired);
                experienceGraph.SetupGraph();
                SectionRect experienceGraphSection = new SectionRect();
                experienceGraphSection.renderAction =
                    delegate (Rect rect)
                    {
                        //Head section.
                        Rect collapsibleRect = rect;
                        collapsibleRect.height = experienceGraphSection.collapsedHeight;

                        DivineJobsUI.CollapsibleSectionHeader(collapsibleRect, experienceGraphSection, "DivineJobs_Tab_Jobs_ExperienceGraph".Translate());

                        if (!experienceGraphSection.isCollapsed)
                        {
                            rect.height -= collapsibleRect.height;
                            rect.y += collapsibleRect.height;

                            rect.width = rect.height;
                            rect.x += rect.width / 2f;

                            experienceGraph.OnGUI(rect);
                        }
                    };
                experienceGraphSection.heightGetter =
                    delegate ()
                    {
                        float result = DivineJobsUI.RowHeight + 256f;
                        return result;
                    };
                jobSections.Add(experienceGraphSection);
            }
        }

        /// <summary>
        /// Draws the default Job tab on GUI.
        /// </summary>
        /// <param name="tabRect">Area the Job tab got to work with.</param>
        public virtual void JobGUITab(Rect tabRect, bool isSelected)
        {
            //Display at a glance information.
            if(isSelected)
            {
                Widgets.DrawBoxSolid(tabRect, highlightColor); 
            }
            else
            {
                Widgets.DrawBoxSolid(tabRect, Widgets.WindowBGFillColor);
            }

            Color color = GUI.color;
            if (isSelected)
            {
                GUI.color = highlightLineColor;
            }

            Widgets.DrawBox(tabRect);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(tabRect, def.LabelCap.WrapInBrackets());

            Text.Anchor = TextAnchor.LowerCenter;
            Widgets.Label(tabRect, $"{level + 1} / {def.maxLevel}");

            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = color;
        }

        public virtual void Notify_JobGUIReset()
        {
            jobSections = null;
            experienceGraph = null;
            //if(jobSections != null)
            {
                //jobSections.Clear();
                /*foreach (SectionRect section in jobSections)
                {
                    section.ResetCache();
                }*/
            }
        }

        /// <summary>
        /// In case the Job needs to draw extra bits on the owner.
        /// </summary>
        public virtual void PostDraw()
        {

        }
    }
}
