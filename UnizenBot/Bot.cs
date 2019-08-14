using Discord;
using UnizenBot.Commands;
using UnizenBot.Integrations.Chat;
using UnizenBot.Integrations.Chat.Discord;
using UnizenBot.Integrations.Chat.Messages;
using UnizenBot.Meta;
using UnizenBot.Plugins;
using UnizenBot.Storage;
using UnizenBot.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnizenBot
{
    /// <summary>
    /// The primary bot class.
    /// </summary>
    public class Bot
    {
        /// <summary>
        /// The path to the bot's configuration file.
        /// </summary>
        public string ConfigPath;

        /// <summary>
        /// The bot configuration.
        /// </summary>
        public BotSettings Settings;

        /// <summary>
        /// A list of all active connections.
        /// </summary>
        public List<IChatConnection> Connections;

        /// <summary>
        /// Handles getting and parsing all Denizen meta.
        /// </summary>
        public MetaHandler Meta;

        /// <summary>
        /// Loads bot plugins.
        /// </summary>
        public PluginLoader<IBotPlugin> Plugins;

        /// <summary>
        /// The folder that plugins should be placed into.
        /// </summary>
        public readonly string PluginsFolder = "plugins";

        /// <summary>
        /// The current Discord connection.
        /// </summary>
        public DiscordConnection DiscordConnection;

        /// <summary>
        /// All valid bot commands.
        /// </summary>
        public Dictionary<string, Func<BotCommand, Task>> Commands;

        /// <summary>
        /// Creates a new bot instance.
        /// </summary>
        public Bot()
        {
        }

        /// <summary>
        /// Configures the bot.
        /// </summary>
        /// <param name="configPath">The path to the bot's configuration file.</param>
        /// <param name="settings">The settings to use.</param>
        public void Configure(string configPath, BotSettings settings)
        {
            ConfigPath = configPath;
            Settings = settings;
            Connections = new List<IChatConnection>();
        }

        /// <summary>
        /// Starts the bot asynchronously.
        /// </summary>
        public async Task Start()
        {
            Console.WriteLine("Fetching meta repositories...");
            if (Directory.Exists("git"))
            {
                FileHelper.NormalizeAttributes("git");
                Directory.Delete("git", true);
            }
            Meta = new MetaHandler(Settings.Meta);
            Commands = new Dictionary<string, Func<BotCommand, Task>>();
            Directory.CreateDirectory(PluginsFolder);
            Console.WriteLine("Loading plugins.");
            Plugins = new PluginLoader<IBotPlugin>();
            foreach (IBotPlugin plugin in Plugins.Load(PluginsFolder))
            {
                plugin.Load(this);
            }
            Console.WriteLine("Reloading meta.");
            Meta.Reload();
            if (Settings.Chat.Discord.Enabled)
            {
                DiscordConnection = new DiscordConnection();
                DiscordConnection.Configure(this, Settings.Chat.Discord);
                Connections.Add(DiscordConnection);
                Console.WriteLine("Beginning asynchronous Discord connection.");
                _ = DiscordConnection.ConnectAsync();
            }
            Console.WriteLine("Setting up command system.");
            RegisterCommands(typeof(Bot).Assembly);
            Console.WriteLine("Successful startup.");
            await Task.Delay(-1);
        }

        /// <summary>
        /// Loads all methods marked with <see cref="CommandHandlerAttribute"/>s in an assembly.
        /// </summary>
        /// <param name="assembly">The assembly to load from.</param>
        public void RegisterCommands(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    try
                    {
                        CommandHandlerAttribute attribute = method.GetCustomAttribute<CommandHandlerAttribute>();
                        if (attribute != null)
                        {
                            Func<BotCommand, Task> func = (Func<BotCommand, Task>)method.CreateDelegate(typeof(Func<BotCommand, Task>));
                            foreach (string alias in attribute.Aliases)
                            {
                                Commands.Add(alias.ToLower(), func);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to register command: " + e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// Handles an incoming user message.
        /// </summary>
        /// <param name="userMessage">The message.</param>
        public async Task HandleUserMessage(AbstractSimpleUserMessage userMessage)
        {
            string text = userMessage.GetSimpleText();
            int cmdIndex = 0;
            if (text.StartsWith("//") || text.StartsWith("!!"))
            {
                cmdIndex = 2;
            }
            else if (text.StartsWith("/") || text.StartsWith("!"))
            {
                cmdIndex = 1;
            }
            else if (!userMessage.HasMentionPrefix(out cmdIndex))
            {
                return;
            }
            text = text.Substring(cmdIndex).TrimStart();
            string[] cmdArgs = text.Split(' ', 2);
            string command = cmdArgs[0].ToLower();
            if (Commands.TryGetValue(command, out Func<BotCommand, Task> func))
            {
                string arguments = cmdArgs.Length > 1 ? cmdArgs[1].Trim() : string.Empty;
                string[] splitArgs = arguments.Split(' ').Where((x) => !string.IsNullOrWhiteSpace(x)).ToArray();
                await func(new BotCommand(this, userMessage, command, splitArgs));
            }
        }

        /// <summary>
        /// Gives basic bot information.
        /// </summary>
        [CommandHandler("info")]
        public static async Task BasicInfo(BotCommand command)
        {
            await command.ReplyAsync(new SimpleMessage(command.Bot.Settings.General.Info));
        }

        /// <summary>
        /// Handles a search for Denizen meta.
        /// </summary>
        /// <typeparam name="T">The meta type.</typeparam>
        /// <param name="searchInput">The search input.</param>
        /// <param name="command">The command that initiated this search.</param>
        /// <param name="listOnly">Whether to only allow a list of results, rather than replying with exact or only matches.</param>
        /// <returns>Whether any results were found.</returns>
        public async Task HandleSearch<T>(string searchInput, BotCommand command, bool listOnly = false) where T : IDenizenMetaType
        {
            searchInput = searchInput.ToLower();
            if (string.IsNullOrWhiteSpace(searchInput))
            {
                return;
            }
            if (searchInput == "all")
            {
                List<Embed> pages = command.Bot.Meta.AllOf<T>().Paginate((tag) => tag.GetListString())
                    .Select((page) => new EmbedBuilder().WithColor(Color.Gold).WithTitle("All known " + Meta.KnownMetaTypeNames[typeof(T)] + "s").WithDescription(page).Build())
                    .ToList();
                await command.ReplyAsync(new DiscordPaginatedMessage(pages));
                return;
            }
            if (searchInput.StartsWith('\\'))
            {
                searchInput = searchInput.Substring(1);
            }
            IEnumerable<SearchResult<T>> results = Meta.Search<T>(searchInput).OrderByDescending((x) => x.MatchLevel);
            SearchResult<T> first = results.FirstOrDefault();
            if (first.Result == null)
            {
                Type type = typeof(T);
                string error;
                if (type == typeof(IDenizenMetaType))
                {
                    error = "Nothing was found matching the specified input. :pensive:";
                }
                else
                {
                    error = "No " + Meta.KnownMetaTypeNames[typeof(T)] + "s were found matching the specified input. :pensive:";
                }
                await command.ReplyAsync(new DiscordEmbedMessage(new EmbedBuilder().WithColor(Color.Red).WithDescription(error).Build()));
                return;
            }
            else if (!listOnly && (first.MatchLevel == SearchMatchLevel.EXACT || results.ElementAtOrDefault(1).Result == null))
            {
                await command.ReplyAsync(new DenizenMetaMessage(first.Result, first.MatchLevel));
                return;
            }
            else
            {
                List<Embed> pages = new List<Embed>();
                EmbedBuilder builder = new EmbedBuilder().WithColor(Color.Gold);
                SearchMatchLevel lastLevel = SearchMatchLevel.NONE;
                EmbedFieldBuilder field = null;
                string value = string.Empty;
                int length = 0;
                int pageLength = 0;
                foreach (SearchResult<T> result in results)
                {
                    if (result.MatchLevel != lastLevel)
                    {
                        if (field != null)
                        {
                            builder.AddField(field.WithValue(value.Substring(0, length - 2)));
                            pageLength += length;
                            value = string.Empty;
                            length = 0;
                        }
                        field = new EmbedFieldBuilder().WithName(MetaCommands.AdaptMatchLevelPlural(result.MatchLevel));
                        lastLevel = result.MatchLevel;
                        if (pageLength > 1250)
                        {
                            pages.Add(builder.Build());
                            builder = new EmbedBuilder().WithColor(Color.Gold);
                        }
                    }
                    string next = result.Result.GetListString() + ", ";
                    int newLength = next.Length + length;
                    if (newLength > 1000 || newLength + pageLength > 1500)
                    {
                        builder.AddField(field.WithValue(value.Substring(0, length - 2)));
                        pages.Add(builder.Build());
                        builder = new EmbedBuilder().WithColor(Color.Gold);
                        field = new EmbedFieldBuilder().WithName(MetaCommands.AdaptMatchLevelPlural(result.MatchLevel) + " [Continued]");
                        pageLength = 0;
                        value = next;
                        length = next.Length;
                    }
                    else
                    {
                        value += next;
                        length = newLength;
                    }
                }
                builder.AddField(field.WithValue(value.Substring(0, length - 2)));
                pages.Add(builder.Build());
                await command.ReplyAsync(new DiscordPaginatedMessage(pages));
                return;
            }
        }
    }
}
