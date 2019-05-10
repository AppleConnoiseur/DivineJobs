using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace DivineJobs.Core
{
    public class ITab_DivineJobs : ITab
    {
        private static readonly Vector2 WinSize = new Vector2(800f, 600f);

        protected override bool StillValid => Find.Selector.SingleSelectedThing is Pawn pawn && SelectedIsColonist && pawn.TryGetComp<DivineJobsComp>() != null;

        public bool SelectedIsColonist
        {
            get
            {
                return SelPawn.IsColonistPlayerControlled;
            }
        }

        public override bool IsVisible => StillValid && SelectedIsColonist;

        public DivineJobsComp JobsComp
        {
            get
            {
                Pawn pawn = Find.Selector.SingleSelectedThing as Pawn;
                return pawn.TryGetComp<DivineJobsComp>();
            }
        }

        public ITab_DivineJobs()
        {
            size = WinSize;
            labelKey = "DivineJobs_TabName";
        }

        public enum JobTabType
        {
            Jobs,
            PossibleJobs,
            Abilities,
            Resources,
            Summary
        }

        //Job selection
        public Pawn currentSelectedPawn = null;
        public JobTabType currentJobTab = JobTabType.Jobs;
        public JobData selectedJob = null;
        public Vector2 leftScrollPosition = new Vector2();

        //Job explorer
        public DivineJobDef selectedPossibleJobDef = null;
        public List<DivineJobDef> availableJobs = new List<DivineJobDef>();
        public Vector2 jobExplorerScrollPosition = new Vector2();
        public GraphView experienceGraph = new GraphView();
        public int refreshCounterAvailableJobs = 0;
        //Filtering
        public bool filterOnlyAvailableJobs = true;
        public string filterByName = "";
        //public List<DivineJobTagDef> filterByTags = new List<DivineJobTagDef>();
        public DivineJobTagDef filterByTag = null;

        //Job information
        public Vector2 jobExplorerInformationScrollPosition = new Vector2();
        public static List<SectionRect> jobExplorerSections = null;

        //Summary
        public Vector2 summaryScrollPosition = new Vector2();

        public override void OnOpen()
        {
            base.OnOpen();

            ResetTab();
            currentSelectedPawn = null;
            leftScrollPosition = new Vector2();

            selectedPossibleJobDef = null;
            jobExplorerScrollPosition = new Vector2();
            jobExplorerInformationScrollPosition = new Vector2();

            summaryScrollPosition = new Vector2();
        }

        public override void Notify_ClearingAllMapsMemory()
        {
            base.Notify_ClearingAllMapsMemory();

            ResetTab();
            currentSelectedPawn = null;
        }

        public void ResetTab()
        {
            currentJobTab = JobTabType.Jobs;
            if(selectedJob != null)
            {
                selectedJob.Notify_JobGUIReset();
            }
            selectedJob = null;
            //filterByTag = null;
            //filterByTags.Clear();
            jobExplorerSections = null;
            selectedPossibleJobDef = null;
            RefreshAvailableJobs();
        }

        public void RefreshAvailableJobs()
        {
            availableJobs.Clear();

            if(Current.Game?.CurrentMap != null)
            {
                DivineJobsComp jobsComp = JobsComp;

                foreach (DivineJobDef jobDef in DefDatabase<DivineJobDef>.AllDefs)
                {
                    //Exclude jobs we already got.
                    if (!jobsComp.jobs.Any(job => job.def == jobDef))
                    {
                        //Include jobs with no requirements or valid requirements.
                        if (jobDef.jobRequirements.Count <= 0 || jobDef.jobRequirements.All(jobReq => jobReq.IsRequirementMet(jobDef, jobsComp, SelPawn)))
                        {
                            availableJobs.Add(jobDef);
                        }
                    }
                }
            }
        }

        protected override void FillTab()
        {
            if(currentSelectedPawn != SelPawn)
            {
                currentSelectedPawn = SelPawn;
                ResetTab();
            }

            if(refreshCounterAvailableJobs >= 200)
            {
                RefreshAvailableJobs();
                refreshCounterAvailableJobs = 0;
            }
            refreshCounterAvailableJobs++;

            //Split the GUI up in three parts.
            //Top: General information and resources.
            //Bottom-Left: Jobs with the Active job and race on top
            //Bottom-Right: Job information and resource information.
            Rect outerRect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(16f);
            float horizontalSplit = (float)Math.Ceiling(outerRect.height * 0.25f);

            Rect generalInformationRect = outerRect;
            generalInformationRect.height = horizontalSplit;
            generalInformationRect = generalInformationRect.ContractedBy(5f);

            Rect bottomSectionRect = outerRect;
            bottomSectionRect.y += horizontalSplit;
            bottomSectionRect.height -= horizontalSplit;
            bottomSectionRect = bottomSectionRect.ContractedBy(5f);

            //Top: General information
            Widgets.DrawMenuSection(generalInformationRect);
            FillGeneralInformation(generalInformationRect.ContractedBy(5f));

            //Bottom: Job and resource information
            //Widgets.DrawMenuSection(bottomSectionRect);
            Text.Font = GameFont.Small;
            bottomSectionRect.y += Text.LineHeight + 5;
            bottomSectionRect.height -= Text.LineHeight + 5;
            FillBottomSection(bottomSectionRect);
        }

        protected void FillGeneralInformation(Rect innerRect)
        {
            DivineJobsComp jobsComp = JobsComp;

            //Tab Layout
            //Active Job                        //Experience and level          //Resources
            //Active Race Job (If possible)     //Experience and level          //Resources
            float verticalSplit = innerRect.width / 3f;
            Rect splitSection = innerRect;
            splitSection.width = verticalSplit;

            //Active Jobs section
            {
                Text.Font = GameFont.Small;

                Rect rowRect = splitSection;
                rowRect.height = Text.LineHeight;

                //Active Job
                Widgets.DrawHighlightIfMouseover(rowRect);
                Widgets.Label(rowRect, $"{"DivineJobs_ActiveJob".Translate()}: {jobsComp.activeJob?.def?.LabelCap?.WrapInBrackets() ?? "N/A"}");
                if(jobsComp.activeJob != null)
                {
                    TooltipHandler.TipRegion(rowRect, jobsComp.activeJob.def.description);
                }
                if(Widgets.ButtonInvisible(rowRect))
                {
                    selectedJob = jobsComp.activeJob;
                    selectedJob.Notify_JobGUIReset();
                    currentJobTab = JobTabType.Jobs;
                }
                rowRect.y += rowRect.height;

                //Race Job
                if(jobsComp.activeRaceJob != null)
                {
                    Widgets.DrawHighlightIfMouseover(rowRect);
                    Widgets.Label(rowRect, $"{"DivineJobs_ActiveRaceJob".Translate()}: {jobsComp.activeRaceJob?.def?.LabelCap?.WrapInBrackets() ?? "????"}");
                    if (jobsComp.activeRaceJob != null)
                    {
                        TooltipHandler.TipRegion(rowRect, jobsComp.activeRaceJob.def.description);
                    }
                    if (Widgets.ButtonInvisible(rowRect))
                    {
                        selectedJob = jobsComp.activeRaceJob;
                        selectedJob.Notify_JobGUIReset();
                        currentJobTab = JobTabType.Jobs;
                    }
                }
            }
            splitSection.x += verticalSplit;

            //Experience and level section
            {
                Text.Font = GameFont.Small;

                Rect rowRect = splitSection;
                rowRect.height = Text.LineHeight;

                //Active Job
                if (jobsComp.activeJob != null)
                {
                    DrawExperienceAndLevelForJob(rowRect, jobsComp.activeJob);
                }
                rowRect.y += rowRect.height;

                //Race Job
                if (jobsComp.activeRaceJob != null)
                {
                    DrawExperienceAndLevelForJob(rowRect, jobsComp.activeRaceJob);
                }
            }
            splitSection.x += verticalSplit;

            //Resources section
            {
                if (jobsComp.resources.Count > 0)
                {

                }
                else
                {
                    Rect rowRect = splitSection;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Text.Font = GameFont.Medium;

                    Widgets.Label(rowRect, "DivineJobs_Tab_NoResources".Translate());

                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Small;
                }
            }
        }

        protected void DrawExperienceAndLevelForJob(Rect innerRect, JobData job)
        {
            //Fillable bar
            Widgets.DrawBoxSolid(innerRect, Widgets.WindowBGFillColor);
            float xpPercent = (float)job.ExperiencePercentToNextLevel;
            Rect fillBar = innerRect;
            fillBar.width = fillBar.width * xpPercent;
            Widgets.DrawBoxSolid(fillBar, Color.blue);
            Widgets.DrawBox(innerRect);

            //Text
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(innerRect, "DivineJobs_JobLevel".Translate(job.level + 1, xpPercent.ToStringPercent()));
            Widgets.DrawHighlightIfMouseover(innerRect);
            if(job.IsFullyLeveled)
            {
                TooltipHandler.TipRegion(innerRect, "DivineJobs_JobLevel_ExperienceMaxed".Translate());
            }
            else
            {
                TooltipHandler.TipRegion(innerRect, $"{Math.Floor(job.experience)} / {job.def.ExperienceRequiredForLevel(job.level + 1)}");
            }

            //Reset
            Text.Anchor = TextAnchor.UpperLeft;
        }

        protected void FillBottomSection(Rect innerRect)
        {
            DivineJobsComp jobsComp = JobsComp;

            Text.Font = GameFont.Small;

            //Tabs for navigation.
            //Rect tabsRect = innerRect;
            //tabsRect.height = Text.LineHeight;

            Widgets.DrawMenuSection(innerRect);
            List<TabRecord> tabRecords = new List<TabRecord>();
            tabRecords.Add(new TabRecord("DivineJobs_Tab_Jobs".Translate(), delegate()
            {
                currentJobTab = JobTabType.Jobs;
            }, currentJobTab == JobTabType.Jobs));
            tabRecords.Add(new TabRecord("DivineJobs_Tab_PossibleJobs".Translate(), delegate ()
            {
                currentJobTab = JobTabType.PossibleJobs;
            }, currentJobTab == JobTabType.PossibleJobs));
            if(jobsComp.resources.Count > 0)
            {
                tabRecords.Add(new TabRecord("DivineJobs_Tab_Resources".Translate(), delegate ()
                {
                    currentJobTab = JobTabType.Resources;
                }, currentJobTab == JobTabType.Resources));
            }
            tabRecords.Add(new TabRecord("DivineJobs_Tab_Summary".Translate(), delegate ()
            {
                currentJobTab = JobTabType.Summary;
            }, currentJobTab == JobTabType.Summary));
            TabDrawer.DrawTabs(innerRect, tabRecords);
            innerRect = innerRect.ContractedBy(9f);
            GUI.BeginGroup(innerRect);
            
            switch(currentJobTab)
            {
                case JobTabType.Jobs:
                    FillJobsTab(innerRect);
                    break;
                case JobTabType.PossibleJobs:
                    FillPossibleJobsTab(innerRect);
                    break;
                case JobTabType.Resources:
                    FillResourcesTab(innerRect);
                    break;
                case JobTabType.Summary:
                    FillSummaryTab(innerRect);
                    break;
            }

            GUI.EndGroup();
        }

        protected void FillResourcesTab(Rect innerRect)
        {
            DivineJobsComp jobsComp = JobsComp;

            //Split into two sections
            //Left: Filter options
            //Right: All available resources.
            innerRect.x = 0f;
            innerRect.y = 0f;
            float verticalSplit = innerRect.width / 3f;
            Rect leftSection = innerRect;
            leftSection.width = verticalSplit;

            Rect rightSection = innerRect;
            rightSection.x += leftSection.width;
            rightSection.width -= leftSection.width;

            leftSection = leftSection.ContractedBy(5f);
            rightSection = rightSection.ContractedBy(5f);
        }

        protected void FillSummaryTab(Rect innerRect)
        {
            DivineJobsComp jobsComp = JobsComp;

            //Show all cached information.
            //Calculate height
            innerRect.x = 0f;
            innerRect.y = 0f;
            float totalHeight = 0f;

            float stageModifierHeight = DivineJobsUI.CalculateHeightOfStageModifier(jobsComp);
            totalHeight += stageModifierHeight;

            //Render summaryScrollPosition
            Rect inRect = innerRect;
            inRect.height = totalHeight;

            Widgets.BeginScrollView(innerRect, ref summaryScrollPosition, inRect);

            Rect rowRect = innerRect;
            /*rowRect.x = 0f;
            rowRect.y = 0f;*/
            rowRect.height = stageModifierHeight;

            DivineJobsUI.FillStageModifier(rowRect, jobsComp, false, null);

            Widgets.EndScrollView();
        }

        protected void FillPossibleJobsTab(Rect innerRect)
        {
            DivineJobsComp jobsComp = JobsComp;
            //Split into two sections
            //Left: Filter options
            //Right: All available jobs.
            innerRect.x = 0f;
            innerRect.y = 0f;
            float verticalSplit = innerRect.width / 3f;
            Rect leftSection = innerRect;
            leftSection.width = verticalSplit;

            Rect rightSection = innerRect;
            rightSection.x += leftSection.width;
            rightSection.width -= leftSection.width;

            leftSection = leftSection.ContractedBy(5f);
            rightSection = rightSection.ContractedBy(5f);

            //Left section
            Widgets.DrawMenuSection(leftSection);
            leftSection = leftSection.ContractedBy(5f);
            //Filters
            {
                Rect rowRect = leftSection;
                rowRect.height = DivineJobsUI.RowHeight;

                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rowRect, "DivineJobs_Filter".Translate());
                rowRect.y += rowRect.height;
                Text.Anchor = TextAnchor.UpperLeft;

                //Only available jobs
                Widgets.CheckboxLabeled(rowRect, "DivineJobs_Filter_AvailableJobsOnly".Translate(), ref filterOnlyAvailableJobs);
                rowRect.y += rowRect.height;

                //Only jobs by name.
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(rowRect, "DivineJobs_Filter_ByName".Translate());
                rowRect.y += rowRect.height;
                Text.Anchor = TextAnchor.UpperLeft;

                filterByName = Widgets.TextField(rowRect, filterByName);
                rowRect.y += rowRect.height;

                //Only jobs by tags.
                //filterByTags
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(rowRect, "DivineJobs_Filter_ByTags".Translate());
                rowRect.y += rowRect.height;
                Text.Anchor = TextAnchor.UpperLeft;

                string tagButtonText = "DivineJobs_None".Translate(); //filterByTag
                if(filterByTag != null)
                {
                    tagButtonText = filterByTag.LabelCap;
                }

                if (Widgets.ButtonText(rowRect, tagButtonText))
                {
                    //Make float menu
                    List<FloatMenuOption> options = new List<FloatMenuOption>();

                    options.Add(new FloatMenuOption("DivineJobs_None".Translate(), 
                        delegate()
                        {
                            filterByTag = null;
                        }));

                    foreach(DivineJobTagDef tagDef in DefDatabase<DivineJobTagDef>.AllDefs.Where(def => !def.hidden))
                    {
                        options.Add(new FloatMenuOption(tagDef.LabelCap,
                        delegate ()
                        {
                            filterByTag = tagDef;
                        }));
                    }

                    FloatMenu floatMenu = new FloatMenu(options);
                    Find.WindowStack.Add(floatMenu);
                }
                rowRect.y += rowRect.height;
            }

            //Right section
            Widgets.DrawMenuSection(rightSection);

            //Two possible states
            //Exploration mode
            //Job information mode
            if(selectedPossibleJobDef != null)
            {
                //Job information mode
                FillJobInformationRect(rightSection.ContractedBy(5f));
            }
            else
            {
                //Exploration mode
                FillExplorationRect(rightSection.ContractedBy(5f));
            }
        }

        public void SetupJobInformationSections(Rect jobRect)
        {
            if (jobExplorerSections == null)
            {
                jobExplorerSections = new List<SectionRect>();
                //
                //  Requirements
                //
                if(selectedPossibleJobDef.jobRequirements.Count > 0)
                {
                    SectionRect requirementsSection = new SectionRect();
                    requirementsSection.renderAction =
                        delegate (Rect rect)
                        {
                        //Head section.
                        Rect collapsibleRect = rect;
                            collapsibleRect.height = requirementsSection.collapsedHeight;

                            DivineJobsUI.CollapsibleSectionHeader(collapsibleRect, requirementsSection, "DivineJobs_JobRequirementReport".Translate());

                            if (!requirementsSection.isCollapsed)
                            {
                                Rect rowRect = collapsibleRect;
                                rowRect.y += rowRect.height;

                                DivineJobsComp jobsComp = JobsComp;
                                foreach (JobRequirementWorker req in selectedPossibleJobDef.jobRequirements)
                                {
                                    string explanation = req.RequirementExplanation(selectedPossibleJobDef, jobsComp, SelPawn);
                                    rowRect.height = Text.CalcHeight(explanation, jobRect.width);
                                    Color color = GUI.color;
                                    if (!req.IsRequirementMet(selectedPossibleJobDef, jobsComp, SelPawn))
                                    {
                                        GUI.color = Color.red;
                                    }
                                    Widgets.Label(rowRect, explanation);
                                    GUI.color = color;
                                    rowRect.y += rowRect.height;
                                }
                            }
                        };
                    requirementsSection.heightGetter =
                        delegate ()
                        {
                            DivineJobsComp jobsComp = JobsComp;
                            float result = DivineJobsUI.RowHeight;
                            foreach (JobRequirementWorker req in selectedPossibleJobDef.jobRequirements)
                            {
                                result += Text.CalcHeight(req.RequirementExplanation(selectedPossibleJobDef, jobsComp, SelPawn), jobRect.width);
                            }
                            return result;
                        };
                    jobExplorerSections.Add(requirementsSection);
                }

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
                            foreach (DivineJobsLevelStage stage in selectedPossibleJobDef.levelStages)
                            {
                                //Stage Level
                                rect.height = DivineJobsUI.RowHeight;
                                
                                
                                Widgets.DrawBoxSolid(rect, Color.cyan);
                                
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
                                DivineJobsUI.FillStageModifier(rect, stage, stageInt == 0, SelPawn);
                                rect.y += stageHeight;
                                stageInt++;
                            }
                        }
                    };
                levelModifierSection.heightGetter =
                    delegate ()
                    {
                        float result = DivineJobsUI.RowHeight;
                        foreach (DivineJobsLevelStage stage in selectedPossibleJobDef.levelStages)
                        {
                            result += DivineJobsUI.RowHeight + DivineJobsUI.CalculateHeightOfStageModifier(stage);
                        }
                        return result;
                    };
                jobExplorerSections.Add(levelModifierSection);

                //
                //  Experience graph -- experienceGraph
                //
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
                jobExplorerSections.Add(experienceGraphSection);
            }
        }

        protected void FillJobInformationRect(Rect innerRect)
        {
            DivineJobsComp jobsComp = JobsComp;

            Rect inRect = innerRect;
            inRect.height = 32f;
            Rect sectionRect = innerRect;
            SetupJobInformationSections(innerRect);
            foreach(SectionRect section in jobExplorerSections)
            {
                inRect.height += section.Height;
            }

            //Calculate height
            //Title
            string titleText = selectedPossibleJobDef.LabelCap.WrapInBrackets();
            Text.Font = GameFont.Medium;
            float titleHeight = Text.CalcHeight(titleText, sectionRect.width);

            inRect.height += titleHeight;

            //Top section: Space for icon, description and job adoption buttons.
            Text.Font = GameFont.Small;
            float descriptionHeight = Text.CalcHeight(selectedPossibleJobDef.description, sectionRect.width);

            inRect.height += descriptionHeight;
            inRect.height += DivineJobsUI.RowHeight;

            //Render
            Widgets.BeginScrollView(innerRect, ref jobExplorerInformationScrollPosition, inRect);

            //Title
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            sectionRect.height = titleHeight;
            Widgets.Label(sectionRect, titleText);
            sectionRect.y += sectionRect.height;

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            sectionRect.height = descriptionHeight;

            //Description
            Widgets.Label(sectionRect, selectedPossibleJobDef.description);
            sectionRect.y += sectionRect.height;

            //Job adoption
            sectionRect.y += DivineJobsUI.RowHeight;
            sectionRect.height = DivineJobsUI.RowHeight;
            string adoptJobString = "DivineJobs_Tab_Jobs_AdoptJob".Translate();
            Vector2 adoptJobTextSize = Text.CalcSize(adoptJobString);
            Rect jobAdoptionButtonRect = sectionRect.CenterOfRectForRect(adoptJobTextSize);
            jobAdoptionButtonRect.width = adoptJobTextSize.x;
            jobAdoptionButtonRect.height = adoptJobTextSize.y;

            if (currentSelectedPawn.CanAdoptJob(selectedPossibleJobDef) && Widgets.ButtonText(jobAdoptionButtonRect.ExpandedBy(3f), adoptJobString))
            {
                //Add job and set it as active.
                jobsComp.AddJob(selectedPossibleJobDef, true);

                //Go to Jobs screen
                currentJobTab = JobTabType.Jobs;
                JobData newJob = null;
                if(jobsComp.TryGetJob(selectedPossibleJobDef, out newJob))
                {
                    selectedJob = newJob;
                    selectedJob.Notify_JobGUIReset();
                }
            }

            sectionRect.y += DivineJobsUI.RowHeight;

            //Job information
            foreach (SectionRect section in jobExplorerSections)
            {
                sectionRect.height = section.Height;
                section.renderAction(sectionRect);
                sectionRect.y += sectionRect.height;
            }

            Widgets.EndScrollView();

            //-- Outside
            //Close button
            const float buttonDimensions = 24f;
            Rect closeButtonRect = new Rect(0f, 0f, buttonDimensions, buttonDimensions);
            closeButtonRect.x = innerRect.x;
            closeButtonRect.y = innerRect.y;

            if(Widgets.ButtonText(closeButtonRect, "X"))
            {
                selectedPossibleJobDef = null;
            }
        }

        protected void FillExplorationRect(Rect innerRect)
        {
            DivineJobsComp jobsComp = JobsComp;

            Rect inRecT = innerRect;
            inRecT.height = 0f;
            IEnumerable<DivineJobDef> candidateJobs = null;

            //Apply filters
            //Only available jobs
            if (filterOnlyAvailableJobs)
            {
                candidateJobs = availableJobs;
            }
            else
            {
                candidateJobs = DefDatabase<DivineJobDef>.AllDefs;
            }

            //By name
            if (!filterByName.NullOrEmpty())
            {
                candidateJobs = candidateJobs.Where(jobDef => jobDef.label.ToLower().Contains(filterByName.ToLower()));
            }

            //By tags -- filterByTags
            /*if(filterByTags.Count > 0)
            {
                candidateJobs = candidateJobs.Where(jobDef => jobDef.tags.Count(tag => filterByTags.Contains(tag)) == filterByTags.Count);
            }*/

            //By tag
            if(filterByTag != null)
            {
                candidateJobs = candidateJobs.Where(jobDef => jobDef.tags.Contains(filterByTag));
            }

            //Calculate inRect size
            int jobsCount = candidateJobs?.Count() ?? 0;
            const int jobColumns = 3;
            const int jobRows = 4;
            float jobRectWidth = innerRect.width / jobColumns;
            float jobRectHeight = innerRect.height / jobRows;
            //Log.Message($"jobsCount={jobsCount}");
            if (jobsCount > 0)
            {
                inRecT.height = jobRectHeight * (float)Math.Ceiling((double)jobsCount / (double)jobColumns);
                //Log.Message($"inRecT.height={inRecT.height}");
            }

            Widgets.BeginScrollView(innerRect, ref jobExplorerScrollPosition, inRecT);
            //Draw possible jobs.
            if (jobsCount > 0)
            {
                int currentColumn = 0;
                int currentRow = 0;

                foreach(DivineJobDef jobDef in candidateJobs)
                {
                    if (currentColumn >= jobColumns)
                    {
                        currentColumn = 0;
                        currentRow++;
                    }

                    Rect jobRect = new Rect(inRecT.x + (jobRectWidth * currentColumn), inRecT.y + (jobRectHeight * currentRow), jobRectWidth, jobRectHeight);
                    jobRect = jobRect.ContractedBy(5f);
                    bool isSelected = selectedPossibleJobDef == jobDef;

                    //Draw info
                    if (isSelected)
                    {
                        Widgets.DrawBoxSolid(jobRect, DivineJobsUI.highlightColor);
                    }
                    else
                    {
                        Widgets.DrawBoxSolid(jobRect, Widgets.WindowBGFillColor);
                    }

                    Color color = GUI.color;
                    if (isSelected)
                    {
                        GUI.color = DivineJobsUI.highlightLineColor;
                    }

                    Widgets.DrawBox(jobRect);

                    Widgets.DrawHighlightIfMouseover(jobRect);

                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(jobRect, jobDef.LabelCap.WrapInBrackets());

                    Text.Anchor = TextAnchor.UpperLeft;
                    GUI.color = color;

                    if(Mouse.IsOver(jobRect))
                    {
                        TooltipHandler.TipRegion(jobRect, $"{jobDef.description}\n\n{DivineJobsUI.GenerateJobRequirementReport(jobDef, jobsComp, SelPawn)}");
                    }

                    if(Widgets.ButtonInvisible(jobRect))
                    {
                        jobExplorerSections = null;
                        selectedPossibleJobDef = jobDef;
                        experienceGraph.targetData = jobDef.ExperiencePerLevelRequired;
                        experienceGraph.SetupGraph();
                    }

                    currentColumn++;
                }
            }

            Widgets.EndScrollView();
        }

        protected void FillJobsTab(Rect innerRect)
        {
            DivineJobsComp jobsComp = JobsComp;
            //Split into two sections.
            //Left: Jobs with the Active job and race on top
            //Right: Job information and resource information.
            innerRect.x = 0f;
            innerRect.y = 0f;
            float verticalSplit = innerRect.width / 3f;
            Rect leftSection = innerRect;
            leftSection.width = verticalSplit;

            Rect rightSection = innerRect;
            rightSection.x += leftSection.width;
            rightSection.width -= leftSection.width;

            leftSection = leftSection.ContractedBy(5f);
            rightSection = rightSection.ContractedBy(5f);

            //Left section
            Widgets.DrawMenuSection(leftSection);
            int jobItems = 0;
            if(jobsComp.activeJob != null)
            {
                jobItems++;
            }
            if (jobsComp.activeRaceJob != null)
            {
                jobItems++;
            }
            jobItems += jobsComp.jobs.Count(jobData => jobData != jobsComp.activeJob && jobData != jobsComp.activeRaceJob);
            leftSection = leftSection.ContractedBy(5f);
            Rect leftInnerScrollRect = leftSection;

            float rowHeight = leftSection.height / 5f;
            Rect leftRowRect = leftInnerScrollRect;
            leftRowRect.height = rowHeight;

            leftInnerScrollRect.height = rowHeight * (float)jobItems;

            Widgets.BeginScrollView(leftSection, ref leftScrollPosition, leftInnerScrollRect);

            //Begin with Active Job and Race Job.
            if (jobsComp.activeJob != null)
            {
                jobsComp.activeJob.JobGUITab(leftRowRect.ContractedBy(5f), selectedJob == jobsComp.activeJob);
                Widgets.DrawHighlightIfMouseover(leftRowRect);
                if(Widgets.ButtonInvisible(leftRowRect))
                {
                    selectedJob = jobsComp.activeJob;
                    selectedJob.Notify_JobGUIReset();
                }
                leftRowRect.y += rowHeight;
            }

            if (jobsComp.activeRaceJob != null)
            {
                jobsComp.activeRaceJob.JobGUITab(leftRowRect.ContractedBy(5f), selectedJob == jobsComp.activeRaceJob);
                Widgets.DrawHighlightIfMouseover(leftRowRect);
                if (Widgets.ButtonInvisible(leftRowRect))
                {
                    selectedJob = jobsComp.activeRaceJob;
                    selectedJob.Notify_JobGUIReset();
                }
                leftRowRect.y += rowHeight;
            }

            foreach(JobData jobData in jobsComp.jobs)
            {
                if(jobData != jobsComp.activeJob && jobData != jobsComp.activeRaceJob)
                {
                    jobData.JobGUITab(leftRowRect.ContractedBy(5f), selectedJob == jobData);
                    Widgets.DrawHighlightIfMouseover(leftRowRect);
                    if (Widgets.ButtonInvisible(leftRowRect))
                    {
                        selectedJob = jobData;
                        selectedJob.Notify_JobGUIReset();
                    }
                    leftRowRect.y += rowHeight;
                }
            }

            Widgets.EndScrollView();

            //Right section
            Widgets.DrawMenuSection(rightSection);
            if(selectedJob != null)
            {
                selectedJob.JobGUI(rightSection.ContractedBy(5f));
            }
            else
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Text.Font = GameFont.Medium;
                Widgets.Label(rightSection.ContractedBy(5f), "DivineJobs_Tab_NoJobSelected".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }
        }
    }
}
