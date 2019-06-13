using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Integrations.Chat.Discord
{
    /// <summary>
    /// Represents a Discord embed message.
    /// </summary>
    public class DiscordEmbedMessage : IMessage
    {
        /// <summary>
        /// The internal Discord embed.
        /// </summary>
        public Embed Internal;

        /// <summary>
        /// Creates a new Discord embed message.
        /// </summary>
        /// <param name="embed">The embed to use.</param>
        public DiscordEmbedMessage(Embed embed)
        {
            Internal = embed;
        }
    }
}
