using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace DivineJobs.Core
{
    public enum ExperienceDistributionPolicy
    {
        ActiveJob,
        ActiveRace,
        SplitBetweenRaceAndJob
    }

    public class DivineJobsComp : ThingComp, IJobStageModifiers
    {
        //Working values
        public List<JobData> jobs = new List<JobData>();
        public List<JobResource> resources = new List<JobResource>();
        public List<AbilityData> abilities = new List<AbilityData>();
        public JobData activeJob;
        public JobData activeRaceJob;
        public ExperienceDistributionPolicy experienceDistributionPolicy;
        /// <summary>
        /// On level up a perk point is gained.
        /// </summary>
        public int perkPoints;

        //Cached values
        private List<StatModifier> cachedStatOffsets = new List<StatModifier>();
        private List<PawnCapacityModifier> cachedCapacityModifiers = new List<PawnCapacityModifier>();
        private List<SkillRequirement> cachedSkillMaxLevels = new List<SkillRequirement>();

        public IEnumerable<StatModifier> StatOffsets
        {
            get
            {
                return cachedStatOffsets.AsReadOnly();
            }
        }

        public IEnumerable<PawnCapacityModifier> CapacityModifiers
        {
            get
            {
                return cachedCapacityModifiers.AsReadOnly();
            }
        }

        public IEnumerable<SkillRequirement> SkillMaxLevels
        {
            get
            {
                return cachedSkillMaxLevels.AsReadOnly();
            }
        }

        public IEnumerable<DivineJobsLevelStage> AllJobStages
        {
            get
            {
                foreach(JobData job in jobs)
                {
                    DivineJobsLevelStage stage = job.GetCurrentJobStage();
                    if(stage != null)
                    {
                        yield return stage;
                    }
                }

                yield break;
            }
        }

        public DivineJobsComp()
        {
            experienceDistributionPolicy = ExperienceDistributionPolicy.SplitBetweenRaceAndJob;
            RefreshCachedModifiers();
        }

        public override void CompTick()
        {
            foreach(JobData job in jobs)
            {
                job.Tick();
            }

            foreach (JobResource resource in resources)
            {
                resource.Tick();
            }

            foreach (AbilityData abilty in abilities)
            {
                abilty.Tick();
            }
        }

        public void RefreshCachedModifiers()
        {
            //Clear all cached values.
            cachedStatOffsets.Clear();
            cachedCapacityModifiers.Clear();
            cachedSkillMaxLevels.Clear();

            if (jobs.Count <= 0)
            {
                return;
            }

            //Recache values.
            foreach (StatModifier statMod in AllJobStages.SelectMany(stage => stage.statOffsets))
            {
                if (cachedStatOffsets.FirstOrDefault(mod => mod.stat == statMod.stat) is StatModifier existingMod)
                {
                    existingMod.value += statMod.value;
                }
                else
                {
                    cachedStatOffsets.Add(
                    new StatModifier()
                    {
                        stat = statMod.stat,
                        value = statMod.value
                    });
                }
            }

            foreach (PawnCapacityModifier capMod in AllJobStages.SelectMany(stage => stage.capacityModifiers))
            {
                if (cachedCapacityModifiers.FirstOrDefault(mod => mod.capacity == capMod.capacity) is PawnCapacityModifier existingMod)
                {
                    existingMod.offset += capMod.offset;
                    existingMod.postFactor += capMod.postFactor;
                }
                else
                {
                    cachedCapacityModifiers.Add(
                    new PawnCapacityModifier()
                    {
                        capacity = capMod.capacity,
                        offset = capMod.offset,
                        postFactor = capMod.postFactor
                    });
                }
            }

            foreach (SkillRequirement skillRequirement in AllJobStages.SelectMany(stage => stage.skillMaxLevels))
            {
                if (cachedSkillMaxLevels.FirstOrDefault(skillReq => skillReq.skill == skillRequirement.skill) is SkillRequirement existingSkill)
                {
                    existingSkill.minLevel = Math.Max(existingSkill.minLevel, skillRequirement.minLevel);
                    //Log.Message($"{parent.LabelCap}: existingSkill.skill={existingSkill.skill.LabelCap}; existingSkill.minLevel={existingSkill.minLevel}");
                }
                else
                {
                    //Log.Message($"{parent.LabelCap}: skillRequirement.skill={skillRequirement.skill.LabelCap}; skillRequirement.minLevel={skillRequirement.minLevel}");
                    cachedSkillMaxLevels.Add(
                        new SkillRequirement()
                        {
                            skill = skillRequirement.skill,
                            minLevel = skillRequirement.minLevel
                        });
                }
            }
        }

        public bool TryGetJob(DivineJobDef def, out JobData jobData)
        {
            if(jobs.FirstOrDefault(job => job.def == def) is JobData foundData)
            {
                jobData = foundData;
                return true;
            }

            jobData = null;
            return false;
        }

        public bool TryGetJob(Predicate<JobData> pred, out JobData jobData)
        {
            if (jobs.FirstOrDefault(job => pred(job)) is JobData foundData)
            {
                jobData = foundData;
                return true;
            }

            jobData = null;
            return false;
        }

        public bool TryGetAbility(DivineAbilityDef def, out AbilityData abilityData)
        {
            if (abilities.FirstOrDefault(ability => ability.def == def) is AbilityData foundData)
            {
                abilityData = foundData;
                return true;
            }

            abilityData = null;
            return false;
        }

        public bool TryGetAbility(Predicate<AbilityData> pred, out AbilityData abilityData)
        {
            if (abilities.FirstOrDefault(ability => pred(ability)) is AbilityData foundData)
            {
                abilityData = foundData;
                return true;
            }

            abilityData = null;
            return false;
        }

        public void AddJob(JobData job)
        {
            jobs.Add(job);
            job.owner = parent as Pawn;
            job.jobsComp = this;
            job.Notify_AcquireThisJob();
            if (activeJob == null && job.def.jobType == JobType.Normal)
            {
                activeJob = job;
            }
            if (activeRaceJob == null && job.def.jobType == JobType.Race)
            {
                activeRaceJob = job;
            }

            RefreshCachedModifiers();
        }

        public void AddJob(DivineJobDef jobDef, bool setAsActive = false)
        {
            JobData job = DivineJobUtility.MakeJobInstance(jobDef);
            AddJob(job);
            if(setAsActive)
            {
                if(job.def.jobType == JobType.Normal)
                {
                    activeJob = job;
                }
                else if(job.def.jobType == JobType.Race)
                {
                    activeRaceJob = job;
                }
            }
        }

        public void AddResource(JobResource resource)
        {
            resources.Add(resource);
            resource.owner = parent as Pawn;
            resource.jobsComp = this;
            resource.Notify_AcquiredThisResource();

            RefreshCachedModifiers();
        }

        public void AddAbility(AbilityData ability)
        {
            abilities.Add(ability);
            ability.owner = parent as Pawn;
            ability.jobsComp = this;
            ability.Notify_AcquireThisAbility();

            RefreshCachedModifiers();
        }

        public void AddExperience(double amount, ExperienceSource source)
        {
            if(activeJob != null && experienceDistributionPolicy != ExperienceDistributionPolicy.ActiveRace)
            {
                float xpModifier = 1f;
                if(activeRaceJob != null && experienceDistributionPolicy == ExperienceDistributionPolicy.SplitBetweenRaceAndJob)
                {
                    xpModifier = 0.5f;
                }

                activeJob.AddExperience(amount * xpModifier, source);
            }
            if (activeRaceJob != null && experienceDistributionPolicy != ExperienceDistributionPolicy.ActiveJob)
            {
                float xpModifier = 1f;
                if (activeJob != null && experienceDistributionPolicy == ExperienceDistributionPolicy.SplitBetweenRaceAndJob)
                {
                    xpModifier = 0.5f;
                }

                activeRaceJob.AddExperience(amount * xpModifier, source);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe.EnterNode("divineJobs");
            Scribe_Values.Look(ref experienceDistributionPolicy, "experienceDistributionPolicy", ExperienceDistributionPolicy.SplitBetweenRaceAndJob);
            Scribe_Values.Look(ref perkPoints, "perkPoints");
            Scribe_Collections.Look(ref jobs, "jobs", LookMode.Deep);
            Scribe_Collections.Look(ref resources, "resources", LookMode.Deep);
            Scribe_Collections.Look(ref abilities, "abilities", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Pawn pawnOwner = parent as Pawn;
                if(pawnOwner != null)
                {
                    if (jobs != null)
                    {
                        foreach (JobData job in jobs)
                        {
                            job.owner = pawnOwner;
                            job.jobsComp = this;
                        }
                    }

                    if (resources != null)
                    {
                        foreach (JobResource resource in resources)
                        {
                            resource.owner = pawnOwner;
                            resource.jobsComp = this;
                        }
                    }

                    if(abilities != null)
                    {
                        foreach (AbilityData ability in abilities)
                        {
                            ability.owner = pawnOwner;
                            ability.jobsComp = this;
                        }
                    }
                }

                //activeJob
                int activeJobIndex = -1;
                Scribe_Values.Look(ref activeJobIndex, "activeJob", -1);
                if(activeJobIndex != -1)
                {
                    activeJob = jobs[activeJobIndex];
                }

                //activeRaceJob
                int activeRaceJobIndex = -1;
                Scribe_Values.Look(ref activeRaceJobIndex, "activeRaceJob", -1);
                if (activeRaceJobIndex != -1)
                {
                    activeRaceJob = jobs[activeRaceJobIndex];
                }

                RefreshCachedModifiers();
            }
            else if(Scribe.mode == LoadSaveMode.Saving)
            {
                //activeJob
                if (jobs.Count > 0 && activeJob != null)
                {
                    int activeJobIndex = jobs.IndexOf(activeJob);
                    Scribe_Values.Look(ref activeJobIndex, "activeJob", -1);
                }

                //activeRaceJob
                if (jobs.Count > 0 && activeRaceJob != null)
                {
                    int activeJobIndex = jobs.IndexOf(activeRaceJob);
                    Scribe_Values.Look(ref activeJobIndex, "activeRaceJob", -1);
                }
            }
            Scribe.ExitNode();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if(parent.Faction != null && !parent.Faction.IsPlayer)
            {
                yield break;
            }

            foreach (AbilityData ability in abilities)
            {
                foreach(Gizmo gizmo in ability.MakeGizmos())
                {
                    yield return gizmo;
                }
            }
        }

        public override void PostDraw()
        {
            foreach (JobData job in jobs)
            {
                job.PostDraw();
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            //Give default starting Job.
            DivineJobDef defaultJob = DefDatabase<DivineJobDef>.AllDefs.FirstOrDefault(def => def.defaultJob);
            if(defaultJob != null)
            {
                AddJob(DivineJobUtility.MakeJobInstance(defaultJob));
            }
        }
    }
}
