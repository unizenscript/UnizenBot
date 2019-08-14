using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Utilities
{
    /// <summary>
    /// Various enum helper methods.
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// Returns the comparitively lowest of two enum values, using <see cref="IComparable.CompareTo(object)"/>
        /// </summary>
        /// <typeparam name="T">The enum.</typeparam>
        /// <param name="first">The first value.</param>
        /// <param name="second">The second value.</param>
        /// <returns>The lowest of the two.</returns>
        public static T Min<T>(T first, T second) where T : Enum
        {
            return first.CompareTo(second) <= 0 ? first : second;
        }

        /// <summary>
        /// Returns the comparitively highest of two enum values, using <see cref="IComparable.CompareTo(object)"/>
        /// </summary>
        /// <typeparam name="T">The enum.</typeparam>
        /// <param name="first">The first value.</param>
        /// <param name="second">The second value.</param>
        /// <returns>The highest of the two.</returns>
        public static T Max<T>(T first, T second) where T : Enum
        {
            return first.CompareTo(second) >= 0 ? first : second;
        }
    }
}
