using UnizenBot.Integrations.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnizenBot.Commands
{
    /// <summary>
    /// Represents an incoming bot command to execute.
    /// </summary>
    public class BotCommand
    {
        /// <summary>
        /// The bot instance.
        /// </summary>
        public Bot Bot;

        /// <summary>
        /// The user message that initiated this command.
        /// </summary>
        public AbstractSimpleUserMessage Message;

        /// <summary>
        /// The alias used in the request.
        /// </summary>
        public string Alias;

        /// <summary>
        /// All arguments.
        /// </summary>
        public string[] Arguments;

        /// <summary>
        /// Constructs a new bot command to run.
        /// </summary>
        /// <param name="bot">The bot instance.</param>
        /// <param name="message">The user message that initiated this command.</param>
        /// <param name="alias">The alias used in the request.</param>
        /// <param name="arguments">All arguments.</param>
        public BotCommand(Bot bot, AbstractSimpleUserMessage message, string alias, string[] arguments)
        {
            Bot = bot;
            Message = message;
            Alias = alias;
            Arguments = arguments;
        }

        /// <summary>
        /// Replies to the user message without a mention.
        /// </summary>
        /// <param name="message">The message to reply with.</param>
        public async Task ReplyAsync(IMessage message)
        {
            await Message.ReplyAsync(message);
        }
    }
}
