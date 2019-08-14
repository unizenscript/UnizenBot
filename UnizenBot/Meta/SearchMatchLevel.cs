using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Meta
{
    /// <summary>
    /// Defines how closely matched two sets of data are.
    /// </summary>
    public enum SearchMatchLevel : int
    {
        /// <summary>
        /// No real similarities could be identified.
        /// </summary>
        NONE,
        /// <summary>
        /// The input could have been intended to be this data.
        /// </summary>
        DID_YOU_MEAN,
        /// <summary>
        /// The data should only be used as a backup.
        /// </summary>
        BACKUP,
        /// <summary>
        /// The data only partially matches.
        /// </summary>
        PARTIAL,
        /// <summary>
        /// The data is relatively similar.
        /// </summary>
        SIMILAR,
        /// <summary>
        /// The data is very similar.
        /// </summary>
        VERY_SIMILAR,
        /// <summary>
        /// The data matches exactly.
        /// </summary>
        EXACT
    }
}
