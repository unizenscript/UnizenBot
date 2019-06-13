using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Plugins
{
    /// <summary>
    /// Represents a loadable bot plugin.
    /// </summary>
    public interface IBotPlugin
    {
        /// <summary>
        /// Loads the plugin. This will be called a single time, on startup.
        /// </summary>
        /// <param name="bot">The current bot instance.</param>
        void Load(Bot bot);
    }
}
