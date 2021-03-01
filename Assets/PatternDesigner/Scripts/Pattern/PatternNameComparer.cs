using System.Collections.Generic;

namespace Assets.PatternDesigner.Scripts.Pattern
{
    /// <summary>
    ///     Compares patterns by their name, alphabetically.
    /// </summary>
    public class PatternNameComparer : IComparer<Pattern>
    {
        public int Compare(Pattern x, Pattern y)
        {
            return string.CompareOrdinal(x?.name, y?.name);
        }
    }
}