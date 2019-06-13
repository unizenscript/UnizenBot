using UnizenBot.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnizenBot.Meta
{
    /// <summary>
    /// Handles getting and parsing all meta.
    /// </summary>
    public class MetaHandler
    {
        /// <summary>
        /// The current meta settings.
        /// </summary>
        public MetaSettings Settings;

        /// <summary>
        /// All registered Denizen meta types.
        /// </summary>
        public Dictionary<string, Type> KnownMetaTypes;

        /// <summary>
        /// The parameterless constructors of registered Denizen meta types.
        /// </summary>
        public Dictionary<Type, ConstructorInfo> MetaTypeConstructors;

        /// <summary>
        /// All property infos of registered Denizen meta types.
        /// </summary>
        public Dictionary<Type, Dictionary<string, MetaPropertyAttribute>> MetaTypeProperties;

        /// <summary>
        /// All known Denizen meta.
        /// </summary>
        public Dictionary<Type, List<IDenizenMetaType>> KnownMeta;

        /// <summary>
        /// Creates a new meta handler.
        /// </summary>
        /// <param name="settings">The settings to use.</param>
        public MetaHandler(MetaSettings settings)
        {
            Settings = settings;
            KnownMetaTypes = new Dictionary<string, Type>();
            MetaTypeConstructors = new Dictionary<Type, ConstructorInfo>();
            MetaTypeProperties = new Dictionary<Type, Dictionary<string, MetaPropertyAttribute>>();
            KnownMeta = new Dictionary<Type, List<IDenizenMetaType>>();
            RegisterDefaultTypes();
        }

        /// <summary>
        /// Registers all default Denizen types.
        /// </summary>
        private void RegisterDefaultTypes()
        {
            RegisterMetaType<DenizenCommand>("command");
            RegisterMetaType<DenizenTag>("tag");
            RegisterMetaType<DenizenMechanism>("mechanism");
            RegisterMetaType<DenizenEvent>("event");
            RegisterMetaType<DenizenAction>("action");
            RegisterMetaType<DenizenLanguage>("language");
        }

        private static readonly Type[] NO_TYPES = new Type[0];

        private static readonly object[] NO_OBJECTS = new object[0];

        /// <summary>
        /// Registers a Denizen meta type with the specified name.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <param name="name">The name of the type. ( // &lt;--[name] )</param>
        public void RegisterMetaType<T>(string name) where T : IDenizenMetaType
        {
            Type type = typeof(T);
            ConstructorInfo constructor = type.GetConstructor(NO_TYPES);
            if (constructor == null)
            {
                throw new InvalidOperationException($"{type.Name} ({name}) does not provide a public parameterless constructor.");
            }
            MetaTypeConstructors.Add(type, constructor);
            Dictionary<string, MetaPropertyAttribute> properties = new Dictionary<string, MetaPropertyAttribute>();
            foreach (PropertyInfo property in type.GetProperties())
            {
                MetaPropertyAttribute meta = property.GetCustomAttribute<MetaPropertyAttribute>();
                if (meta != null && typeof(ISingleOrList<string>).IsAssignableFrom(property.PropertyType))
                {
                    meta.PropertyInfo = property;
                    properties.Add(meta.Name?.ToLower() ?? property.Name.ToLower(), meta);
                }
            }
            MetaTypeProperties.Add(type, properties);
            KnownMetaTypes.Add(name.ToLower(), type);
        }

        private object ReloadLock = new object();

        /// <summary>
        /// Reloads all meta.
        /// </summary>
        public void Reload()
        {
            lock (ReloadLock)
            {
                _Reload();
            }
        }

        /// <summary>
        /// Retrieves all of the meta of the specified type. If <see cref="IDenizenMetaType"/> is provided as the type, returns all meta.
        /// <para>Generally, you should prefer <see cref="AllOf{T}"/>.</para>
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns>All meta of the specified type, or all meta if <see cref="IDenizenMetaType"/> is provided as the type.</returns>
        public IEnumerable<IDenizenMetaType> AllOf(Type type)
        {
            if (type == typeof(IDenizenMetaType))
            {
                foreach (IDenizenMetaType meta in KnownMeta.Values.SelectMany((x) => x))
                {
                    yield return meta;
                }
            }
            else
            {
                foreach (IDenizenMetaType meta in KnownMeta[type])
                {
                    yield return meta;
                }
            }
        }

        /// <summary>
        /// Retrieves all of the meta of the specified type. If <see cref="IDenizenMetaType"/> is provided as the type, returns all meta.
        /// </summary>
        /// <typeparam name="T">The type to search for.</typeparam>
        /// <returns>All meta of the specified type, or all meta if <see cref="IDenizenMetaType"/> is provided as the type.</returns>
        public IEnumerable<T> AllOf<T>() where T : IDenizenMetaType
        {
            foreach (IDenizenMetaType meta in AllOf(typeof(T)))
            {
                yield return (T)meta;
            }
        }

        /// <summary>
        /// Searches meta of the specified type and returns any matches more successful than <see cref="SearchMatchLevel.NONE"/>.
        /// <para>If <see cref="IDenizenMetaType"/> is provided as the type, searches all meta.</para>
        /// </summary>
        /// <typeparam name="T">The type to search for.</typeparam>
        /// <param name="input">The string search.</param>
        /// <returns>Any and all matching meta of the specified type.</returns>
        public IEnumerable<SearchResult<T>> Search<T>(string input) where T : IDenizenMetaType
        {
            input = input.ToLower();
            foreach (T meta in AllOf<T>())
            {
                SearchMatchLevel match = meta.Matches(input);
                if (match != SearchMatchLevel.NONE)
                {
                    yield return new SearchResult<T>()
                    {
                        Result = meta,
                        MatchLevel = match
                    };
                }
            }
        }

        private void _Reload()
        {
            Directory.CreateDirectory("git");
            KnownMeta = new Dictionary<Type, List<IDenizenMetaType>>();
            foreach (Type type in KnownMetaTypes.Values)
            {
                KnownMeta.Add(type, new List<IDenizenMetaType>());
            }
            foreach (RepositorySettings repository in Settings.Repositories)
            {
                string repoName = repository.Url.EndsWith('/') ? repository.Url.Substring(0, repository.Url.Length - 1) : repository.Url;
                repoName = repoName.EndsWith(".git") ? repoName.Substring(0, repoName.Length - ".git".Length) : repoName;
                repoName = repoName.Substring(repoName.LastIndexOf('.') + 1);
                repoName = repoName.Substring(repoName.IndexOf('/') + 1);
                CloneRepo(repository.Url, "git/" + repoName, repository.Checkout, repository.Username, repository.AccessToken);
            }
            List<string> reloadLog = new List<string>();
            foreach (MetaFileSettings fileSettings in Settings.Files)
            {
                foreach (string file in Directory.GetFiles("git/", "*." + fileSettings.Extension, SearchOption.AllDirectories))
                {
                    Load(file, fileSettings.Delimiter, reloadLog);
                }
            }
            File.WriteAllLines("reload.log", reloadLog);
        }

        private void CloneRepo(string repo, string destination, string checkout = null, string username = null, string accessToken = null)
        {
            if (username != null)
            {
                repo = $"https://{username}:{accessToken}@{repo.Replace("https://", "").Replace("http://", "")}";
            }
            bool needsCheckout = false;
            string arguments;
            if (Directory.Exists(destination))
            {
                arguments = $"-C {destination} pull";
            }
            else
            {
                arguments = $"clone {repo} {destination} --depth 1";
                if (checkout != null)
                {
                    arguments += " --no-checkout --filter=blob:none";
                    needsCheckout = true;
                }
            }
            RunProcess(Settings.GitPath, arguments);
            if (needsCheckout)
            {
                RunProcess(Settings.GitPath, $"-C {destination} checkout {checkout}");
            }
        }

        private string RunProcess(string file, string arguments)
        {
            string output = string.Empty;
            using (Process p = new Process())
            {
                p.StartInfo = new ProcessStartInfo()
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = file,
                    Arguments = arguments
                };
                p.Start();
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            return output;
        }

        private void Load(string file, string delimiter, List<string> reloadLog)
        {
            using (StreamReader reader = new StreamReader(file))
            {
                int lineNumber = 0;
                string line;
                Type currentType = null;
                IDenizenMetaType currentMeta = null;
                ISingleOrList<string> currentProperty = null;
                string propertyValue = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (currentType == null)
                    {
                        int index = line.IndexOf(delimiter + " <--[");
                        if (index >= 0)
                        {
                            string type = line.Substring(index + (delimiter + " <--[").Length);
                            index = type.IndexOf(']');
                            if (index >= 0)
                            {
                                type = type.Substring(0, index).ToLower();
                                if (KnownMetaTypes.TryGetValue(type, out currentType))
                                {
                                    currentMeta = (IDenizenMetaType)MetaTypeConstructors[currentType].Invoke(NO_OBJECTS);
                                }
                                else
                                {
                                    reloadLog.Add($"Unknown meta type on line {lineNumber} in file: {file}");
                                    reloadLog.Add($"'{type}'");
                                    reloadLog.Add(line);
                                }
                            }
                            else
                            {
                                reloadLog.Add($"Invalid opening meta formatting on line {lineNumber} in file: {file}");
                                reloadLog.Add(line);
                            }
                        }
                    }
                    else
                    {
                        int index = line.IndexOf(delimiter + " @");
                        bool isEnd = line.IndexOf(delimiter + " -->") >= 0;
                        if ((index >= 0 || isEnd) && currentProperty != null)
                        {
                            try
                            {
                                currentProperty.Add(propertyValue.Trim());
                            }
                            catch (InvalidOperationException)
                            {
                                reloadLog.Add($"Invalid multi-section meta on line {lineNumber} in file: {file}");
                                reloadLog.Add(line);
                            }
                            currentProperty = null;
                            propertyValue = string.Empty;
                        }
                        if (isEnd)
                        {
                            KnownMeta[currentType].Add(currentMeta);
                            currentType = null;
                            currentMeta = null;
                        }
                        else if (index >= 0)
                        {
                            string property = line.Substring(index + (delimiter + " @").Length).Trim();
                            index = property.IndexOf(' ');
                            if (index >= 0)
                            {
                                propertyValue += property.Substring(index + 1) + "\n";
                                property = property.Substring(0, index);
                            }
                            if (MetaTypeProperties[currentType].TryGetValue(property.ToLower(), out MetaPropertyAttribute propertyAttribute))
                            {
                                PropertyInfo propertyInfo = propertyAttribute.PropertyInfo;
                                currentProperty = (ISingleOrList<string>)propertyInfo.GetValue(currentMeta);
                                if (currentProperty == null)
                                {
                                    currentProperty = (ISingleOrList<string>)propertyInfo.PropertyType.GetConstructor(NO_TYPES).Invoke(NO_OBJECTS);
                                    propertyInfo.SetValue(currentMeta, currentProperty);
                                }
                            }
                            else
                            {
                                reloadLog.Add($"Unknown {currentType.Name} property on line {lineNumber} in file: {file}");
                                reloadLog.Add(line);
                                propertyValue = string.Empty;
                            }
                        }
                        else if (currentProperty != null)
                        {
                            index = line.IndexOf(delimiter + " ");
                            if (index >= 0)
                            {
                                propertyValue += line.Substring(index + (delimiter + " ").Length) + "\n";
                            }
                        }
                    }
                }
            }
        }
    }
}
