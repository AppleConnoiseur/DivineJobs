using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    /// <summary>
    /// From what source the experience came from.
    /// </summary>
    public enum ExperienceSource
    {
        /// <summary>
        /// Special source. Makes all sources equal.
        /// </summary>
        General,
        Combat,
        Crafting,
        Harvesting,
        Mining,
        Intellectual
    }

    /// <summary>
    /// Determines what type of Job this is. They are handled a bit differently.
    /// </summary>
    public enum JobType
    {
        Normal,
        Race
    }

    /// <summary>
    /// Defines a Divine Job.
    /// </summary>
    public class DivineJobDef : Def
    {
        /// <summary>
        /// Worker class instance for the Job.
        /// </summary>
        public Type jobClass = typeof(JobData);

        /// <summary>
        /// Maximum level this Job can get to.
        /// </summary>
        public int maxLevel = 10;

        /// <summary>
        /// The experience needed per level to advance to the next level.
        /// </summary>
        public MathGraph experienceCurve = new ExponentialGraph();

        /// <summary>
        /// The stage of each level for the Job. 
        /// </summary>
        public List<DivineJobsLevelStage> levelStages = new List<DivineJobsLevelStage>();

        /// <summary>
        /// What type of Job this is.
        /// </summary>
        public JobType jobType = JobType.Normal;

        /// <summary>
        /// All these requirements must be met in order to be able to pick this Job.
        /// </summary>
        public List<JobRequirementWorker> jobRequirements = new List<JobRequirementWorker>();

        /// <summary>
        /// This Job get the full experience from this source. Half from any other.
        /// </summary>
        public ExperienceSource preferredExperienceSource = ExperienceSource.General;

        /// <summary>
        /// What this Job is tagged with. Useful for grouping it Jobs into groups.
        /// </summary>
        public List<DivineJobTagDef> tags = new List<DivineJobTagDef>();

        /// <summary>
        /// This Job is given to anyone regardless of background.
        /// </summary>
        public bool defaultJob = false;

        //Variables to assist in Job exploration. Should not be manually set in XML.
        /// <summary>
        /// Jobs it are linked from backward.
        /// </summary>
        [Unsaved]
        public List<DivineJobDef> linkedJobsFrom = new List<DivineJobDef>();
        /// <summary>
        /// Jobs it link to forward.
        /// </summary>
        [Unsaved]
        public List<DivineJobDef> linkedJobsTo = new List<DivineJobDef>();

        /// <summary>
        /// For debugging purposes only.
        /// </summary>
        public bool debugShowExperienceInConsole = false;

        private List<double> totaltExperienceNeededToAdvance;

        public IEnumerable<double> ExperiencePerLevelRequired
        {
            get
            {
                return totaltExperienceNeededToAdvance.AsReadOnly();
            }
        }

        public double ExperienceRequiredForLevel(int level)
        {
            return totaltExperienceNeededToAdvance[level];
        }

        public override void PostLoad()
        {
            base.PostLoad();

            //Cache calculations
            totaltExperienceNeededToAdvance = new List<double>(maxLevel);
            double previousExperienceNeeded = 0d;
            for (int i = 0; i < maxLevel; i++)
            {
                totaltExperienceNeededToAdvance.Add(Math.Floor(previousExperienceNeeded));
                previousExperienceNeeded += experienceCurve.Convert(i);
            }

            //Debug
            if(debugShowExperienceInConsole)
            {
                Log.Message($"Total experience needed to advance in Job '{defName}':");
                int i = 1;
                foreach (double exp in totaltExperienceNeededToAdvance)
                {
                    Log.Message($"Level {i} => {(int)exp}");
                    i++;
                }
            }
        }
    }
}
