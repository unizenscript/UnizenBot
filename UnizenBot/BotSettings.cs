using UnizenBot.Integrations.Chat.Discord;
using UnizenBot.Meta;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot
{
    /// <summary>
    /// The bot settings structure.
    /// </summary>
    public class BotSettings
    {
        /// <summary>
        /// Holds all chat settings.
        /// </summary>
        public ChatSettings Chat { get; set; }

        /// <summary>
        /// Holds all meta settings.
        /// </summary>
        public MetaSettings Meta { get; set; }

        /// <summary>
        /// Various other settings.
        /// </summary>
        public GeneralSettings General { get; set; }
    }

    /// <summary>
    /// The bot chat settings structure.
    /// </summary>
    public class ChatSettings
    {
        /// <summary>
        /// <see cref="DiscordConnection"/> settings.
        /// </summary>
        public DiscordSettings Discord { get; set; }
    }

    /// <summary>
    /// Various other settings.
    /// </summary>
    public class GeneralSettings
    {
        /// <summary>
        /// The output of the '!info' command.
        /// </summary>
        public string Info { get; set; }
    }
}
