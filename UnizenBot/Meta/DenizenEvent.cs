﻿using UnizenBot.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace UnizenBot.Meta
{
    /// <summary>
    /// Represents a Denizen event.
    /// </summary>
    public class DenizenEvent : IDenizenMetaType
    {
        /// <summary>
        /// Line-separated list of valid event names.
        /// </summary>
        [MetaProperty(Display = "Events", Position = 0)]
        public SingleObject<string> Events { get; set; }

        /// <summary>
        /// The valid switches for the event.
        /// </summary>
        [MetaProperty(Display = "Switch", Position = 1, Inline = true)]
        public ListObject<string> Switch { get; set; }

        /// <summary>
        /// Description of when the event triggers.
        /// </summary>
        [MetaProperty(Display = "Triggers", Position = 2)]
        public SingleObject<string> Triggers { get; set; }

        /// <summary>
        /// Line-separated list of context tags.
        /// </summary>
        [MetaProperty(Display = "Context", Position = 3)]
        public SingleObject<string> Context { get; set; }

        /// <summary>
        /// Line-separated list of possible determinations.
        /// </summary>
        [MetaProperty(Display = "Determine", Position = 4)]
        public SingleObject<string> Determine { get; set; }

        /// <summary>
        /// Whether the event is cancellable.
        /// </summary>
        [MetaProperty(Display = "Cancellable", Position = 5)]
        public SingleObject<string> Cancellable { get; set; }

        /// <summary>
        /// The regex for the event.
        /// </summary>
        public SingleObject<string> Regex { get; set; }

        /// <summary>
        /// Gets a simple string representing this meta for list output.
        /// </summary>
        public string GetListString()
        {
            return "!e " + Events.Value.Split('\n').Stringify((evtName) => evtName, ", !e ");
        }

        private Regex CachedRegex;

        /// <summary>
        /// Checks how well this event matches a string search.
        /// </summary>
        /// <param name="input">The string search.</param>
        /// <returns>How well this event matches a string search.</returns>
        public SearchMatchLevel Matches(string input)
        {
            input = input.ToLower().Trim();
            if (!input.StartsWith("on "))
            {
                input = "on " + input;
            }
            if (Regex != null)
            {
                if (CachedRegex == null)
                {
                    CachedRegex = new Regex(Regex.Value, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
                if (CachedRegex.Matches(input).Count > 0)
                {
                    return SearchMatchLevel.EXACT;
                }
            }
            input = input.Substring("on ".Length);
            SearchMatchLevel tentative = SearchMatchLevel.NONE;
            foreach (string evnt in Events.Value.Split('\n'))
            {
                string evt = evnt.ToLower();
                if (evt == input)
                {
                    return SearchMatchLevel.EXACT;
                }
                else if (evt.StartsWith(input))
                {
                    int lengthDiff = evt.Length - input.Length;
                    if (lengthDiff > 0 && lengthDiff <= 9)
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
