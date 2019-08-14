using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Utilities
{
    /// <summary>
    /// General static utility methods.
    /// </summary>
    public class Util
    {
        /// <summary>
        /// Computes the number of character edits required to transform <paramref name="first"/> into <paramref name="second"/>.
        /// </summary>
        public static int LevenshteinDistance(string first, string second)
        {
            int firstLength = first?.Length ?? 0;
            int secondLength = second?.Length ?? 0;
            if (firstLength == 0)
            {
                return secondLength;
            }
            if (secondLength == 0)
            {
                return firstLength;
            }
            int[,] distances = new int[firstLength + 1, secondLength + 1];
            for (int i = 0; i <= firstLength; i++)
            {
                distances[i, 0] = i;
            }
            for (int j = 0; j <= secondLength; distances[0, j] = j++)
            {
                distances[0, j] = j;
            }
            int cost;
            for (int i = 1; i <= firstLength; i++)
            {
                for (int j = 1; j <= secondLength; j++)
                {
                    cost = (first[i - 1] == second[j - 1]) ? 0 : 1;
                    distances[i, j] = Math.Min(Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1), distances[i - 1, j - 1] + cost);
                }
            }
            return distances[firstLength, secondLength];
        }

        /// <summary>
        /// Returns whether the first string has the same first or last character and that their <see cref="LevenshteinDistance(string, string)"/> is less than or equal to 3.
        /// </summary>
        public static bool IsTextSimilar(string first, string second)
        {
            if (first == null || second == null)
            {
                return false;
            }
            if (first[0] == second[0] || first[first.Length - 1] == second[second.Length - 1])
            {
                return LevenshteinDistance(first, second) <= 3;
            }
            return false;
        }
    }
}
