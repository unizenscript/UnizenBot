using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Meta
{
    /// <summary>
    /// Represents a search result, with the result object and its relevant <see cref="SearchMatchLevel"/>.
    /// </summary>
    public struct SearchResult<T>
    {
        /// <summary>
        /// The result object.
        /// </summary>
        public T Result;

        /// <summary>
        /// How closely the result object matched the search input.
        /// </summary>
        public SearchMatchLevel MatchLevel;
    }
}
