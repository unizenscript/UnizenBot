using UnizenBot.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Meta
{
    /// <summary>
    /// Represents a Denizen mechanism.
    /// </summary>
    public class DenizenMechanism : IDenizenMetaType
    {
        /// <summary>
        /// The object type the mechanism applies to.
        /// </summary>
        [MetaProperty(Display = "Object", Position = 0, Inline = true)]
        public SingleObject<string> Object { get; set; }

        /// <summary>
        /// The name of the mechanism.
        /// </summary>
        [MetaProperty(Display = "Name", Position = 1, Inline = true)]
        public SingleObject<string> Name { get; set; }

        /// <summary>
        /// The input type for the mechanism.
        /// </summary>
        [MetaProperty(Display = "Input", Position = 2, Inline = true)]
        public SingleObject<string> Input { get; set; }

        /// <summary>
        /// The description of the mechanism.
        /// </summary>
        [MetaProperty(Display = "Description", Position = 3)]
        public SingleObject<string> Description { get; set; }

        /// <summary>
        /// A line-separated list of tags related to this mechanism.
        /// </summary>
        [MetaProperty(Display = "Tags", Position = 4)]
        public SingleObject<string> Tags { get; set; }

        /// <summary>
        /// Gets a simple string representing this meta for list output.
        /// </summary>
        public string GetListString()
        {
            return "!m " + Object.Value + "." + Name.Value;
        }

        /// <summary>
        /// Checks how well this mechanism's name matches a string search.
        /// </summary>
        /// <param name="input">The string search.</param>
        /// <returns>How well this mechanism's name matches a string search.</returns>
        public SearchMatchLevel Matches(string input)
        {
            string obj = Object.Value.ToLower();
            if (obj.EndsWith("tag"))
            {
                obj = obj.Substring(0, obj.Length - "tag".Length);
            }
            string name = obj + "." + Name.Value.ToLower();
            input = input.ToLower();
            int tagdot = input.IndexOf("tag.");
            if (tagdot > 0) // somethingtag.blah -> something.blah
            {
                input = input.Substring(0, tagdot) + input.Substring(tagdot + "tag".Length);
            }
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
