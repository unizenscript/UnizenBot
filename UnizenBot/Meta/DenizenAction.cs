using UnizenBot.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Meta
{
    /// <summary>
    /// Represents a Denizen action.
    /// </summary>
    public class DenizenAction : IDenizenMetaType
    {
        /// <summary>
        /// Line-separated list of valid action names.
        /// </summary>
        [MetaProperty(Display = "Actions", Position = 0)]
        public SingleObject<string> Actions { get; set; }

        /// <summary>
        /// Description of when the action triggers.
        /// </summary>
        [MetaProperty(Display = "Triggers", Position = 1)]
        public SingleObject<string> Triggers { get; set; }

        /// <summary>
        /// Line-separated list of context tags.
        /// </summary>
        [MetaProperty(Display = "Context", Position = 2)]
        public SingleObject<string> Context { get; set; }

        /// <summary>
        /// Line-separated list of possible determinations.
        /// </summary>
        [MetaProperty(Display = "Determine", Position = 3)]
        public SingleObject<string> Determine { get; set; }

        /// <summary>
        /// Gets a simple string representing this meta for list output.
        /// </summary>
        public string GetListString()
        {
            return "!a " + Actions.Value.Split('\n').Stringify((actionName) => actionName, ", !a ");
        }

        /// <summary>
        /// Checks how well this action matches a string search.
        /// </summary>
        /// <param name="input">The string search.</param>
        /// <returns>How well this action matches a string search.</returns>
        public SearchMatchLevel Matches(string input)
        {
            input = input.ToLower().Trim();
            if (input.StartsWith("on "))
            {
                input = input.Substring("on ".Length);
            }
            foreach (string evnt in Actions.Value.Split('\n'))
            {
                string evt = evnt.ToLower();
                if (evt == input)
                {
                    return SearchMatchLevel.EXACT;
                }
                else if (evt.StartsWith(input))
                {
                    int lengthDiff = evt.Length - input.Length;
                    if (lengthDiff > 0 && lengthDiff <= 3)
                    {
                        return SearchMatchLevel.VERY_SIMILAR;
                    }
                    else
                    {
                        return SearchMatchLevel.SIMILAR;
                    }
                }
                else if (evt.Contains(input))
                {
                    return SearchMatchLevel.PARTIAL;
                }
            }
            return SearchMatchLevel.NONE;
        }
    }
}
