using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Utilities
{
    /// <summary>
    /// A collection of extensions for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Converts an enumerable to a string list using a specified separator.
        /// </summary>
        /// <typeparam name="T">The result type searched for.</typeparam>
        /// <param name="enumerable">The objects to convert into a string list.</param>
        /// <param name="adapt">Adapts objects to their string form for printing in the list output.</param>
        /// <param name="separator">The separator to use.</param>
        /// <returns>A stringified list.</returns>
        public static string Stringify<T>(this IEnumerable<T> enumerable, Func<T, string> adapt, string separator = ", ")
        {
            string output = string.Empty;
            foreach (T obj in enumerable)
            {
                output += adapt(obj) + separator;
            }
            return output.Substring(0, output.Length - separator.Length);
        }

        /// <summary>
        /// Converts an enumerable to a paginated string list using a specified separator.
        /// </summary>
        /// <typeparam name="T">The result type searched for.</typeparam>
        /// <param name="enumerable">The objects to convert into a string list.</param>
        /// <param name="adapt">Adapts objects to their string form for printing in the list output.</param>
        /// <param name="lengthPerPage">Length per page.</param>
        /// <param name="separator">The separator to use.</param>
        /// <returns>A stringified list.</returns>
        public static List<string> Paginate<T>(this IEnumerable<T> enumerable, Func<T, string> adapt, int lengthPerPage = 1500, string separator = ", ")
        {
            List<string> pages = new List<string>();
            string page = string.Empty;
            foreach (T obj in enumerable)
            {
                string val = adapt(obj);
                string test = page + val + separator;
                if (test.Length < lengthPerPage)
                {
                    page = test;
                }
                else
                {
                    pages.Add(page.Substring(0, page.Length - separator.Length));
                    page = val + separator;
                }
            }
            if (page != string.Empty)
            {
                pages.Add(page.Substring(0, page.Length - separator.Length));
            }
            return pages;
        }
    }
}
