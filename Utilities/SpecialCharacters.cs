using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineJobs.Core
{
    public static class SpecialCharacters
    {
        public static char leftBracket = '【';
        public static char rightBracket = '】';
        public static char passedRequirement = '✓';
        public static char failedRequirement = 'X';

        public static string WrapInBrackets(this string input)
        {
            return $"{leftBracket}{input}{rightBracket}";
        }

        public static char BoolIntoSymbol(this bool input)
        {
            if(input)
            {
                return passedRequirement;
            }
            else
            {
                return failedRequirement;
            }
        }
    }
}
