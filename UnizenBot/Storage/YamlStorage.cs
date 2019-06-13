using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace UnizenBot.Storage
{
    /// <summary>
    /// YAML file-based storage provider.
    /// </summary>
    public class YamlStorage : IStructuredStorage
    {
        /// <summary>
        /// The YAML deserializer.
        /// </summary>
        public Deserializer Deserializer;

        /// <summary>
        /// The YAML serializer.
        /// </summary>
        public Serializer Serializer;

        /// <summary>
        /// Creates a new YAML storage provider with the specified naming convention.
        /// </summary>
        /// <param name="convention">The naming convention to use.</param>
        public YamlStorage(INamingConvention convention)
        {
            Deserializer = new DeserializerBuilder()
                .WithNamingConvention(convention)
                .Build();
            Serializer = new SerializerBuilder()
                .WithNamingConvention(convention)
                .Build();
        }

        /// <summary>
        /// Reads a data structure from the specified stream.
        /// <para>This will not close the stream.</para>
        /// </summary>
        /// <typeparam name="T">The data structure type.</typeparam>
        /// <returns>A data structure, filled if possible.</returns>
        public T Load<T>(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return Deserializer.Deserialize<T>(reader);
            }
        }
    }
}
