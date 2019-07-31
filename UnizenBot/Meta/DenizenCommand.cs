using UnizenBot.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Meta
{
    /// <summary>
    /// Represents a Denizen command.
    /// </summary>
    public class DenizenCommand : IDenizenMetaType
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        [MetaProperty(Display = "Name", Position = 0, Inline = true)]
        public SingleObject<string> Name { get; set; }

        /// <summary>
        /// The number of required arguments.
        /// </summary>
        [MetaProperty(Display = "Args Required", Position = 1, Inline = true)]
        public SingleObject<string> Required { get; set; }

        /// <summary>
        /// The meta group this command is in.
        /// </summary>
        [MetaProperty(Display = "Group", Position = 2, Inline = true)]
        public SingleObject<string> Group { get; set; } = new SingleObject<string>() { Value = "none" };

        /// <summary>
        /// The syntax of the command.
        /// </summary>
        [MetaProperty(Display = "Syntax", Position = 3)]
        public SingleObject<string> Syntax { get; set; }

        /// <summary>
        /// The short description of the command.
        /// </summary>
        [MetaProperty(Display = "Description", Position = 4)]
        public SingleObject<string> Short { get; set; }

        /// <summary>
        /// A line-separated list of tags related to this command.
        /// </summary>
        [MetaProperty(Display = "Tags", Position = 5)]
        public SingleObject<string> Tags { get; set; }

        /// <summary>
        /// The description of the command.
        /// </summary>
        [MetaProperty(Display = "Long Description", Position = 6, ForceNextPage = true)]
        public SingleObject<string> Description { get; set; }

        /// <summary>
        /// The example usages of the command.
        /// </summary>
        [MetaProperty(Display = "Usage", Position = 7, Code = true, ForceNextPage = true, PerPage = 2)]
        public ListObject<string> Usage { get; set; }

        /// <summary>
        /// The original author of this command's code.
        /// </summary>
        [MetaProperty(Display = "Initial Author", Position = 8, ForceNextPage = true)]
        public SingleObject<string> Author { get; set; }

        /// <summary>
        /// Gets a simple string representing this meta for list output.
        /// </summary>
        public string GetListString()
        {
            return Name.Value;
        }

        /// <summary>
        /// Checks how well this command's name matches a string search.
        /// </summary>
        /// <param name="input">The string search.</param>
        /// <returns>How well this command's name matches a string search.</returns>
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
            else
            {
                return SearchMatchLevel.NONE;
            }
        }
    }
}
