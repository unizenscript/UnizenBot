using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Integrations.Chat
{
    /// <summary>
    /// Represents a simple textual message.
    /// </summary>
    public class SimpleMessage : IMessage
    {
        /// <summary>
        /// The simple text.
        /// </summary>
        public string Text;

        /// <summary>
        /// Creates a new simple message.
        /// </summary>
        /// <param name="text">The textual message.</param>
        public SimpleMessage(string text)
        {
            Text = text;
        }
    }
}
