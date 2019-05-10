using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace DivineJobs.Core
{
    /// <summary>
    /// Represents a usable resource for a Job. Like Mana, Stamina, Ki, Souls, Pets or Equipment.
    /// </summary>
    public class JobResource : IExposable
    {
        public Pawn owner;
        public DivineJobsComp jobsComp;

        public DivineJobResourceDef def;
        public bool showInGUI = true;

        public static Vector2 defaultSize = new Vector2(128f, 24f);

        public virtual Vector2 PreferredSize
        {
            get
            {
                return defaultSize;
            }
        }

        public JobResource()
        {

        }

        public virtual void PostMake()
        {

        }

        /// <summary>
        /// Draws the resource GUI. Should be able to adapt to different sizes.
        /// </summary>
        /// <param name="resourceRect">Drawing area the resource got to work with.</param>
        public virtual void OnGUI(Rect resourceRect)
        {

        }

        /// <summary>
        /// In case the Resource needs to draw extra bits on the owner.
        /// </summary>
        public virtual void PostDraw()
        {

        }

        /// <summary>
        /// Ticked when the Pawn ticks.
        /// </summary>
        public virtual void Tick()
        {

        }

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref showInGUI, "showInGUI");
        }

        public virtual void Notify_AcquiredThisResource()
        {

        }
    }
}
