using UnizenBot.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Meta
{
    /// <summary>
    /// Represents a meta-documented Denizen language explanation.
    /// </summary>
    public class DenizenLanguage : IDenizenMetaType
    {
        /// <summary>
        /// The name of the language explanation.
        /// </summary>
        [MetaProperty(Display = "Name", Position = 0, Inline = true)]
        public SingleObject<string> Name { get; set; }

        /// <summary>
        /// The meta group this language explanation is in.
        /// </summary>
        [MetaProperty(Display = "Group", Position = 1, Inline = true)]
        public SingleObject<string> Group { get; set; } = new SingleObject<string>() { Value = "none" };

        /// <summary>
        /// Full description of the language explanation.
        /// </summary>
        [MetaProperty(Display = "Description", Position = 2)]
        public SingleObject<string> Description { get; set; }

        /// <summary>
        /// Gets a simple string representing this meta for list output.
        /// </summary>
        public string GetListString()
        {
            return "!l " + Name.Value;
        }

        /// <summary>
        /// Checks how well this language explanation's name matches a string search.
        /// </summary>
        /// <param name="input">The string search.</param>
        /// <returns>How well this language explanation's name matches a string search.</returns>
        public SearchMatchLevel Matches(string input)
        {
            string name = Name.Value.ToLower();
            if (input == name)
            {
                return SearchMatchLevel.EXACT;
            }
            else if (name.StartsWith(input))
            {
                int lengthDiff = name.Length - input.Length;
                if (lengthDiff > 0 && lengthDiff <= 3)
                {
                    return SearchMatchLevel.VERY_SIMILAR;
                }
                else
                {
                    return SearchMatchLevel.SIMILAR;
                }
            }
            else if (name.Contains(input))
            {
                return SearchMatchLevel.PARTIAL;
            }
            else if (Util.IsTextSimilar(name, input))
            {
                return SearchMatchLevel.DID_YOU_MEAN;
            }
            else
            {
                return SearchMatchLevel.NONE;
            }
        }
    }
}
