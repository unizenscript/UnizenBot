using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace UnizenBot.Plugins
{
    /// <summary>
    /// Handles loading assemblies containing a class that implements an interface <typeparamref name="T"/>.
    /// </summary>
    public class PluginLoader<T>
    {
        /// <summary>
        /// Loads any plugins contained within <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The plugins folder path.</param>
        public IEnumerable<T> Load(string path)
        {
            Type pluginType = typeof(T);
            string[] plugins = Directory.GetFiles(path, "*.dll");
            foreach (string plugin in plugins)
            {
                Assembly assembly = Assembly.LoadFrom(plugin);
                foreach (TypeInfo type in assembly.DefinedTypes)
                {
                    if (type.IsInterface || type.IsAbstract)
                    {
                        continue;
                    }
                    if (type.GetInterface(pluginType.FullName) != null)
                    {
                        yield return (T)Activator.CreateInstance(type);
                    }
                }
            }
        }
    }
}
