using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using UnizenBot.Commands;
using UnizenBot.Integrations.Chat.Messages;
using UnizenBot.Meta;
using UnizenBot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace UnizenBot.Integrations.Chat.Discord
{
    /// <summary>
    /// Handles the connection to Discord.
    /// </summary>
    public class DiscordConnection : IChatConnection<DiscordSettings>
    {
        /// <summary>
        /// The bot instance.
        /// </summary>
        public Bot Bot;

        /// <summary>
        /// The Discord settings.
        /// </summary>
        public DiscordSettings Settings;

        /// <summary>
        /// The current bot client.
        /// </summary>
        public DiscordSocketClient Client;

        /// <summary>
        /// The last paginated message sent in each channel.
        /// </summary>
        public Dictionary<ulong, DiscordPaginatedMessage> LastPaginated = new Dictionary<ulong, DiscordPaginatedMessage>();

        /// <summary>
        /// The last pagination error message sent in each channel.
        /// </summary>
        public Dictionary<ulong, RestUserMessage> LastPageError = new Dictionary<ulong, RestUserMessage>();

        /// <summary>
        /// All paginated messages that have been sent in this session.
        /// </summary>
        public Dictionary<ulong, DiscordPaginatedMessage> PaginatedMessages = new Dictionary<ulong, DiscordPaginatedMessage>();

        /// <summary>
        /// Configures this connection.
        /// </summary>
        /// <param name="bot">The bot instance.</param>
        /// <param name="settings">The settings to use.</param>
        public void Configure(Bot bot, DiscordSettings settings)
        {
            Bot = bot;
            Settings = settings;
        }

        /// <summary>
        /// Begins this connection asynchronously.
        /// </summary>
        public async Task ConnectAsync()
        {
            if (string.IsNullOrWhiteSpace(Settings.BotToken))
            {
                Console.WriteLine($"Missing or empty 'chat.discord.bot_token' key in {Bot.ConfigPath}!");
                return;
            }
            Client = new DiscordSocketClient();
            Client.MessageReceived += HandleMessage;
            Client.ReactionAdded += HandleReaction;
            await Client.LoginAsync(TokenType.Bot, Settings.BotToken);
            await Client.StartAsync();
            await Task.Delay(-1); // TODO: intermittently restart connection if needed
        }

        /// <summary>
        /// Handles any incoming message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task HandleMessage(SocketMessage message)
        {
            if (message is SocketUserMessage userMessage && !message.Author.IsBot)
            {
                await Bot.HandleUserMessage(new DiscordUserMessage(this, userMessage));
            }
        }

        /// <summary>
        /// Go to the first paginated page.
        /// </summary>
        public static readonly Emoji FirstPage = new Emoji("⏮");

        /// <summary>
        /// Go to the previous paginated page.
        /// </summary>
        public static readonly Emoji PreviousPage = new Emoji("⬅");

        /// <summary>
        /// Go to the next paginated page.
        /// </summary>
        public static readonly Emoji NextPage = new Emoji("➡");

        /// <summary>
        /// Go to the last paginated page.
        /// </summary>
        public static readonly Emoji LastPage = new Emoji("⏭");

        /// <summary>
        /// Handles any incoming reactions.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="channel"></param>
        /// <param name="reaction">The reaction.</param>
        public async Task HandleReaction(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (!reaction.User.Value.IsBot && PaginatedMessages.TryGetValue(message.Id, out DiscordPaginatedMessage paginated))
            {
                if (reaction.Emote.Equals(NextPage))
                {
                    if (paginated.CurrentPage < paginated.PageCount - 1)
                    {
                        paginated.CurrentPage++;
                        await paginated.MessageToEdit.ModifyAsync((x) => x.Embed = paginated.GetPage(paginated.CurrentPage));
                    }
                }
                else if (reaction.Emote.Equals(PreviousPage))
                {
                    if (paginated.CurrentPage > 0)
                    {
                        paginated.CurrentPage--;
                        await paginated.MessageToEdit.ModifyAsync((x) => x.Embed = paginated.GetPage(paginated.CurrentPage));
                    }
                }
                else if (reaction.Emote.Equals(FirstPage))
                {
                    if (paginated.CurrentPage > 0)
                    {
                        paginated.CurrentPage = 0;
                        await paginated.MessageToEdit.ModifyAsync((x) => x.Embed = paginated.GetPage(paginated.CurrentPage));
                    }
                }
                else if (reaction.Emote.Equals(LastPage))
                {
                    if (paginated.CurrentPage < paginated.PageCount - 1)
                    {
                        paginated.CurrentPage = paginated.PageCount - 1;
                        await paginated.MessageToEdit.ModifyAsync((x) => x.Embed = paginated.GetPage(paginated.CurrentPage));
                    }
                }
                _ = paginated.MessageToEdit.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            }
        }

        /// <summary>
        /// Creates a new paginated message from meta information.
        /// </summary>
        /// <param name="meta">The meta to paginate.</param>
        /// <returns>A new paginated message.</returns>
        public DiscordPaginatedMessage GetPaginatedMeta(DenizenMetaMessage meta)
        {
            List<Embed> pages = new List<Embed>();
            string title = MetaCommands.AdaptMatchLevel(meta.MatchLevel);
            EmbedBuilder builder = new EmbedBuilder().WithColor(Color.Green).WithTitle(title);
            foreach (MetaPropertyAttribute property in Bot.Meta.MetaTypeProperties[meta.Meta.GetType()].Values.OrderBy((x) => x.Position))
            {
                object propertyValue = property.PropertyInfo.GetValue(meta.Meta);
                if (propertyValue == null)
                {
                    continue;
                }
                if (property.ForceNextPage)
                {
                    AddPage(ref pages, ref builder, title);
                }
                if (propertyValue is SingleObject<string> single)
                {
                    string value = single.Value;
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                    if (property.Code)
                    {
                        value = $"```{property.CodeExtension}\n{value}\n```";
                    }
                    else
                    {
                        value = Regex.Replace(value, "<@link [^\\s]+ ([^>]+)>", "$1");
                    }
                    if (value.Length > 1000)
                    {
                        int pos = 0;
                        int remaining = value.Length;
                        int nextLength = 1000;
                        bool inCode = false;
                        string current;
                        do
                        {
                            current = value.Substring(pos, nextLength);
                            if (nextLength >= 1000)
                            {
                                int cut = current.LastIndexOf('\n');
                                if (cut == -1)
                                {
                                    cut = current.LastIndexOf(' ');
                                }
                                current = current.Substring(0, cut + 1);
                            }
                            pos += current.Length;
                            remaining -= current.Length;
                            nextLength = remaining > 1000 ? 1000 : remaining;
                            ParseCode(ref current, ref inCode, property.CodeExtension);
                            builder.AddField(new EmbedFieldBuilder().WithName(property.Display + (pos > 1000 ? " [Continued]" : "")).WithValue(current));
                            AddPage(ref pages, ref builder, title);
                        }
                        while (remaining > 0);
                    }
                    else
                    {
                        bool inCode = false;
                        ParseCode(ref value, ref inCode, property.CodeExtension);
                        builder.AddField(new EmbedFieldBuilder().WithName(property.Display).WithValue(value).WithIsInline(property.Inline));
                    }
                }
                else if (propertyValue is ListObject<string> list)
                {
                    int count = 0;
                    int total = 0;
                    foreach (string current in list)
                    {
                        string value = current;
                        count++;
                        total++;
                        if (property.Code)
                        {
                            value = $"```{property.CodeExtension}\n{value}\n```";
                        }
                        builder.AddField(new EmbedFieldBuilder().WithName(property.Display + " #" + total).WithValue(value).WithIsInline(property.Inline));
                        if (count == property.PerPage)
                        {
                            AddPage(ref pages, ref builder, title);
                            count = 0;
                        }
                    }
                }
            }
            if (builder.Fields.Count > 0)
            {
                pages.Add(builder.Build());
            }
            return new DiscordPaginatedMessage(pages);
        }

        private void AddPage(ref List<Embed> pages, ref EmbedBuilder builder, string title)
        {
            pages.Add(builder.Build());
            builder = new EmbedBuilder().WithColor(Color.Green).WithTitle(title);
        }

        private void ParseCode(ref string text, ref bool inCode, string extension)
        {
            if (inCode)
            {
                text = "```" + extension + "\n" + text;
            }
            int codeStart = text.IndexOf("<code>");
            if (!inCode && codeStart == -1)
            {
                text = text.Replace(">", "\\>").Replace("[", "\\[").Replace("(", "\\(");
            }
            while (inCode || codeStart >= 0)
            {
                if (!inCode)
                {
                    text = text.Replace("\\>", ">").Replace("\\[", "[").Replace("\\(", "(").Substring(0, codeStart) + "```" + extension + "\n" + text.Substring(codeStart + "<code>".Length);
                }
                int codeEnd = text.IndexOf("</code>");
                if (codeEnd >= 0)
                {
                    text = text.Substring(0, codeEnd) + "\n```" + text.Substring(codeEnd + "</code>".Length).Replace(">", "\\>").Replace("[", "\\[").Replace("(", "\\("); ;
                    codeStart = text.IndexOf("<code>", codeEnd);
                    inCode = false;
                }
                else
                {
                    text += "```";
                    inCode = true;
                    return;
                }
            }
            inCode = false;
        }
    }
}
