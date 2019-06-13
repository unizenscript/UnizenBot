using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Commands
{
    /// <summary>
    /// Marks a useable bot command method with the signature: <code>static Task Command(BotCommand command)</code>
    /// </summary>
    public class CommandHandlerAttribute : Attribute
    {
        /// <summary>
        /// The help response for a failed command.
        /// </summary>
        public string Help;

        /// <summary>
        /// The aliases to allow for this command.
        /// </summary>
        public string[] Aliases;

        /// <summary>
        /// Creates a new bot command attribute.
        /// </summary>
        /// <param name="aliases">All aliases to allow.</param>
        public CommandHandlerAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}
