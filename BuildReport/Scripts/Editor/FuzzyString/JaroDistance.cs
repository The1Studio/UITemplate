using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyString
{
    public static partial class ComparisonMetrics
    {
        public static double JaroDistance(this string source, string target)
        {
            var m = source.Intersect(target).Count();

            if (m == 0)
            {
                return 0;
            }
            else
            {
                var sourceTargetIntersetAsString                                              = "";
                var targetSourceIntersetAsString                                              = "";
                var sourceIntersectTarget                                                     = source.Intersect(target);
                var targetIntersectSource                                                     = target.Intersect(source);
                foreach (var character in sourceIntersectTarget) sourceTargetIntersetAsString += character;
                foreach (var character in targetIntersectSource) targetSourceIntersetAsString += character;
                double t                                                                      = sourceTargetIntersetAsString.LevenshteinDistance(targetSourceIntersetAsString) / 2;
                return (m / source.Length + m / target.Length + (m - t) / m) / 3;
            }
        }
    }
}