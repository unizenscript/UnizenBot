using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnizenBot.Storage
{
    /// <summary>
    /// Represents a type of data storage that supports structured data.
    /// </summary>
    public interface IStructuredStorage
    {
        /// <summary>
        /// Reads a data structure from the specified stream.
        /// <para>This will not close the stream.</para>
        /// </summary>
        /// <typeparam name="T">The data structure type.</typeparam>
        /// <returns>A data structure, filled if possible.</returns>
        T Load<T>(Stream stream);
    }
}
