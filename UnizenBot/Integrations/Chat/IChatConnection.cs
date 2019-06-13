using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnizenBot.Integrations.Chat
{
    /// <summary>
    /// Represents a connection that the bot may use for typical communications.
    /// </summary>
    /// <typeparam name="T">The connection settings structure.</typeparam>
    public interface IChatConnection<T> : IChatConnection
    {
        /// <summary>
        /// Configures this connection.
        /// </summary>
        /// <param name="bot">The bot instance.</param>
        /// <param name="settings">The settings to use.</param>
        void Configure(Bot bot, T settings);

        /// <summary>
        /// Begins this connection asynchronously.
        /// </summary>
        Task ConnectAsync();
    }

    /// <summary>
    /// Represents a connection that the bot may use for typical communications.
    /// </summary>
    public interface IChatConnection
    {
    }
}
