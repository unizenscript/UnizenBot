using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Integrations.Chat.Discord
{
    /// <summary>
    /// Settings structure for <see cref="DiscordConnection"/>.
    /// </summary>
    public class DiscordSettings
    {
        /// <summary>
        /// Whether the Discord connection should be created.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The bot token to connect with.
        /// </summary>
        public string BotToken { get; set; }
    }
}
