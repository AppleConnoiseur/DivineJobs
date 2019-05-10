using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    /// <summary>
    /// Defines the behavior of an ability.
    /// </summary>
    public class DivineAbilityDef : Def
    {
        public Type abilityClass = typeof(AbilityData);

        public GraphicData defaultIconGraphicData;

        [Unsaved]
        public Graphic defaultIconGraphic = BaseContent.BadGraphic;

        public bool passiveAbility = false;

        public bool isViolent = true;

        public bool canHitNonTargetPawns = true;

        public AbilityTargetingWorker targetingWorker;

        public List<AbilityEffectWorker> effectWorkers = new List<AbilityEffectWorker>();

        public VerbProperties verbProperties;

        private List<VerbProperties> verbs;

        public List<VerbProperties> Verbs
        {
            get
            {
                if(verbs == null)
                {
                    verbs = new List<VerbProperties>(1);
                    if(verbProperties != null)
                    {
                        verbs.Add(verbProperties);
                    }
                }

                return verbs;
            }
        }

        public override void PostLoad()
        {
            if (defaultIconGraphicData != null)
            {
                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    if (defaultIconGraphicData.shaderType == null)
                    {
                        defaultIconGraphicData.shaderType = ShaderTypeDefOf.Cutout;
                    }
                    defaultIconGraphic = defaultIconGraphicData.Graphic;
                });
            }
        }
    }
}
