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
            SearchMatchLevel tentative = SearchMatchLevel.NONE;
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
                        tentative = EnumHelper.Max(tentative, SearchMatchLevel.VERY_SIMILAR);
                    }
                    else
                    {
                        tentative = EnumHelper.Max(tentative, SearchMatchLevel.SIMILAR);
                    }
                }
                else if (evt.Contains(input))
                {
                    tentative = EnumHelper.Max(tentative, SearchMatchLevel.PARTIAL);
                }
                else if (Util.IsTextSimilar(evt, input))
                {
                    tentative = EnumHelper.Max(tentative, SearchMatchLevel.DID_YOU_MEAN);
                }
            }
            if (input.Length > 3 && Triggers.Value.ToLower().Contains(input))
            {
                tentative = EnumHelper.Max(tentative, SearchMatchLevel.BACKUP);
            }
            return tentative;
        }
    }
}
