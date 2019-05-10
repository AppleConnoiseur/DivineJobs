using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineJobs.Core
{
    public interface IAbilityComponentExplanation
    {
        string GetComponentExplanation(AbilityData abilityData);
    }
}
