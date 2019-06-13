using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnizenBot.Integrations.Chat
{
    /// <summary>
    /// A simple message from a user that can be responded to per implementation.
    /// </summary>
    public abstract class AbstractSimpleUserMessage : IMessage
    {
        /// <summary>
        /// Whether this message mentions the bot.
        /// </summary>
        public abstract bool HasMentionPrefix(out int argPos);

        /// <summary>
        /// Retrieves the most basic version of this message, compatible with any text-based platforms.
        /// </summary>
        public abstract string GetSimpleText();

        /// <summary>
        /// Replies to the user message without a mention.
        /// </summary>
        /// <param name="message">The message to reply with.</param>
        public abstract Task ReplyAsync(IMessage message);
    }
}
