using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DivineJobs.Core
{
    /// <summary>
    /// Core class for resources like Mana and Ki.
    /// </summary>
    public class JobResource_Numerical : JobResource
    {
        public float amount;
        public float maxAmount;

        public override void Tick()
        {
            //Regenerate amount.
            amount += def.baseValueRegeneration;
            if(amount > maxAmount)
            {
                amount = maxAmount;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref amount, "value");
            Scribe_Values.Look(ref maxAmount, "maxAmount");
        }
    }
}
