using UnizenBot.Meta;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Integrations.Chat.Messages
{
    /// <summary>
    /// Represents a message containing information about a Denizen meta type, to be handled by <see cref="IChatConnection"/>.
    /// </summary>
    public class DenizenMetaMessage : IMessage
    {
        /// <summary>
        /// The meta information.
        /// </summary>
        public IDenizenMetaType Meta;

        /// <summary>
        /// How well the meta matched search input.
        /// </summary>
        public SearchMatchLevel MatchLevel;

        /// <summary>
        /// Creates a new message containing information about a Denizen meta type.
        /// </summary>
        /// <param name="meta">The meta information.</param>
        /// <param name="matchLevel">How well the meta matched search input.</param>
        public DenizenMetaMessage(IDenizenMetaType meta, SearchMatchLevel matchLevel)
        {
            Meta = meta;
            MatchLevel = matchLevel;
        }
    }
}
