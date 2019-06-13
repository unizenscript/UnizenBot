using UnizenBot.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Meta
{
    /// <summary>
    /// Represents a Denizen tag.
    /// </summary>
    public class DenizenTag : IDenizenMetaType
    {
        /// <summary>
        /// The attribute name.
        /// </summary>
        [MetaProperty(Display = "Attribute", Position = 0, Inline = true)]
        public SingleObject<string> Attribute { get; set; }

        /// <summary>
        /// The type this tag returns.
        /// </summary>
        [MetaProperty(Display = "Returns", Position = 1, Inline = true)]
        public SingleObject<string> Returns { get; set; }

        /// <summary>
        /// The meta group this tag is in.
        /// </summary>
        [MetaProperty(Display = "Group", Position = 2, Inline = true)]
        public SingleObject<string> Group { get; set; }

        /// <summary>
        /// The mechanism to modify the tag's value.
        /// </summary>
        [MetaProperty(Display = "Mechanism", Position = 3, Inline = true)]
        public SingleObject<string> Mechanism { get; set; }

        /// <summary>
        /// The description of this tag.
        /// </summary>
        [MetaProperty(Display = "Description", Position = 4)]
        public SingleObject<string> Description { get; set; }

        /// <summary>
        /// Required plugins for this tag.
        /// </summary>
        [MetaProperty(Display = "Required Plugins", Position = 5)]
        public SingleObject<string> Plugin { get; set; }

        /// <summary>
        /// Gets a simple string representing this meta for list output.
        /// </summary>
        public string GetListString()
        {
            return Attribute.Value;
        }

        private string StripTag(string tag)
        {
            tag = tag.Trim();
            if (tag.StartsWith('<'))
            {
                tag = tag.Substring(1);
            }
            if (tag.EndsWith('>'))
            {
                tag = tag.Substring(0, tag.Length - 1);
            }
            int open = tag.IndexOf('[');
            while (open >= 0)
            {
                string first = tag.Substring(0, open);
                string second = tag.Substring(tag.IndexOf(']') + 1);
                tag = first + second;
                open = tag.IndexOf('[');
            }
            return tag.Substring(tag.IndexOf('@') + 1);
        }

        /// <summary>
        /// Checks how well this tag's attribute matches a string search.
        /// </summary>
        /// <param name="input">The string search.</param>
        /// <returns>How well this tag's attribute matches a string search.</returns>
        public SearchMatchLevel Matches(string input)
        {
            input = StripTag(input.ToLower());
            string attribute = StripTag(Attribute.Value.ToLower());
            string[] inSplit = input.Split('.');
            string[] attrSplit = attribute.Split('.', inSplit.Length);
            if (inSplit.Length > 1)
            {
                if (inSplit.Length > attrSplit.Length)
                {
                    return SearchMatchLevel.NONE;
                }
                SearchMatchLevel lowest = SearchMatchLevel.EXACT;
                for (int i = 0; i < inSplit.Length; i++)
                {
                    lowest = EnumHelper.Min(lowest, BasicCheck(inSplit[i], attrSplit[i]));
                }
                return lowest;
            }
            else
            {
                return BasicCheck(input, attribute);
            }
        }

        private SearchMatchLevel BasicCheck(string input, string attribute)
        {
            if (input == attribute)
            {
                return SearchMatchLevel.EXACT;
            }
            else if (attribute.StartsWith(input))
            {
                int lengthDiff = attribute.Length - input.Length;
                if (lengthDiff > 0 && lengthDiff <= 3)
                {
                    return SearchMatchLevel.VERY_SIMILAR;
                }
                else
                {
                    return SearchMatchLevel.SIMILAR;
                }
            }
            else if (attribute.Contains(input))
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
