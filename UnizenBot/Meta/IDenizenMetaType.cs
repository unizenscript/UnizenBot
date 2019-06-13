using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Meta
{
    /// <summary>
    /// Represents a type of Denizen meta documentation.
    /// </summary>
    public interface IDenizenMetaType
    {
        /// <summary>
        /// Checks how well this meta type matches a string search. The definition of matching may vary greatly between types.
        /// </summary>
        /// <param name="input">The string search.</param>
        /// <returns>How well this meta type matches a string search.</returns>
        SearchMatchLevel Matches(string input);

        /// <summary>
        /// Gets a simple string representing this meta for list output.
        /// </summary>
        string GetListString();
    }
}
