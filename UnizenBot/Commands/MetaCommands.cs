using Discord;
using UnizenBot.Integrations.Chat;
using UnizenBot.Integrations.Chat.Discord;
using UnizenBot.Meta;
using UnizenBot.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnizenBot.Commands
{
    /// <summary>
    /// Contains all Denizen meta commands.
    /// </summary>
    public class MetaCommands
    {
        /// <summary>
        /// Searches documented commands.
        /// </summary>
        [CommandHandler("command", "cmd", "c")]
        public static async Task SearchCommands(BotCommand command)
        {
            string searchCmd = command.Arguments[0].ToLower();
            if (string.IsNullOrWhiteSpace(searchCmd))
            {
                return;
            }
            if (searchCmd == "all")
            {
                List<Embed> pages = command.Bot.Meta.AllOf<DenizenCommand>().Paginate((cmd) => cmd.GetListString())
                    .Select((page) => new EmbedBuilder().WithColor(Color.Gold).WithTitle("All known commands").WithDescription(page).Build())
                    .ToList();
                await command.ReplyAsync(new DiscordPaginatedMessage(pages));
                return;
            }
            if (searchCmd.StartsWith('\\'))
            {
                searchCmd = searchCmd.Substring(1);
            }
            if (!await command.Bot.HandleSearch<DenizenCommand>(searchCmd, command))
            {
                await command.ReplyAsync(new DiscordEmbedMessage(new EmbedBuilder().WithColor(Color.Red)
                    .WithDescription("No commands were found matching the specified input. :pensive:").Build()));
            }
        }

        /// <summary>
        /// Searches documented tags.
        /// </summary>
        [CommandHandler("tag", "t")]
        public static async Task SearchTags(BotCommand command)
        {
            string searchTag = command.Arguments[0].ToLower();
            if (searchTag == "all")
            {
                List<Embed> pages = command.Bot.Meta.AllOf<DenizenTag>().Paginate((tag) => tag.GetListString())
                    .Select((page) => new EmbedBuilder().WithColor(Color.Gold).WithTitle("All known tags").WithDescription(page).Build())
                    .ToList();
                await command.ReplyAsync(new DiscordPaginatedMessage(pages));
                return;
            }
            if (searchTag.StartsWith('\\'))
            {
                searchTag = searchTag.Substring(1);
            }
            if (!await command.Bot.HandleSearch<DenizenTag>(searchTag, command))
            {
                await command.ReplyAsync(new DiscordEmbedMessage(new EmbedBuilder().WithColor(Color.Red)
                    .WithDescription("No tags were found matching the specified input. :pensive:").Build()));
            }
        }

        /// <summary>
        /// Searches documented mechanisms.
        /// </summary>
        [CommandHandler("mechanism", "mech", "mec", "m")]
        public static async Task SearchMechanisms(BotCommand command)
        {
            string searchMech = command.Arguments[0].ToLower();
            if (searchMech == "all")
            {
                List<Embed> pages = command.Bot.Meta.AllOf<DenizenMechanism>().Paginate((mech) => mech.GetListString())
                    .Select((page) => new EmbedBuilder().WithColor(Color.Gold).WithTitle("All known mechanisms").WithDescription(page).Build())
                    .ToList();
                await command.ReplyAsync(new DiscordPaginatedMessage(pages));
                return;
            }
            if (searchMech.StartsWith('\\'))
            {
                searchMech = searchMech.Substring(1);
            }
            if (!await command.Bot.HandleSearch<DenizenMechanism>(searchMech, command))
            {
                await command.ReplyAsync(new DiscordEmbedMessage(new EmbedBuilder().WithColor(Color.Red)
                    .WithDescription("No mechanisms were found matching the specified input. :pensive:").Build()));
            }
        }

        /// <summary>
        /// Searches documented events.
        /// </summary>
        [CommandHandler("event", "evt", "e")]
        public static async Task SearchEvents(BotCommand command)
        {
            string searchEvent = command.Arguments.Stringify((x) => x, " ").ToLower();
            if (searchEvent == "all")
            {
                List<Embed> pages = command.Bot.Meta.AllOf<DenizenEvent>().Paginate((evt) => evt.GetListString())
                    .Select((page) => new EmbedBuilder().WithColor(Color.Gold).WithTitle("All known events").WithDescription(page).Build())
                    .ToList();
                await command.ReplyAsync(new DiscordPaginatedMessage(pages));
                return;
            }
            if (searchEvent.StartsWith('\\'))
            {
                searchEvent = searchEvent.Substring(1);
            }
            if (!await command.Bot.HandleSearch<DenizenEvent>(searchEvent, command))
            {
                await command.ReplyAsync(new DiscordEmbedMessage(new EmbedBuilder().WithColor(Color.Red)
                    .WithDescription("No events were found matching the specified input. :pensive:").Build()));
            }
        }

        /// <summary>
        /// Searches documented events.
        /// </summary>
        [CommandHandler("action", "act", "a")]
        public static async Task SearchActions(BotCommand command)
        {
            string searchAction = command.Arguments.Stringify((x) => x, " ").ToLower();
            if (searchAction == "all")
            {
                List<Embed> pages = command.Bot.Meta.AllOf<DenizenAction>().Paginate((action) => action.GetListString())
                    .Select((page) => new EmbedBuilder().WithColor(Color.Gold).WithTitle("All known actions").WithDescription(page).Build())
                    .ToList();
                await command.ReplyAsync(new DiscordPaginatedMessage(pages));
                return;
            }
            if (searchAction.StartsWith('\\'))
            {
                searchAction = searchAction.Substring(1);
            }
            if (!await command.Bot.HandleSearch<DenizenAction>(searchAction, command))
            {
                await command.ReplyAsync(new DiscordEmbedMessage(new EmbedBuilder().WithColor(Color.Red)
                    .WithDescription("No actions were found matching the specified input. :pensive:").Build()));
            }
        }

        /// <summary>
        /// Searches documented language explanations.
        /// </summary>
        [CommandHandler("language", "lang", "lng", "l")]
        public static async Task SearchLanguages(BotCommand command)
        {
            string searchLang = command.Arguments.Stringify((x) => x, " ").ToLower();
            if (string.IsNullOrWhiteSpace(searchLang))
            {
                return;
            }
            if (searchLang == "all")
            {
                List<Embed> pages = command.Bot.Meta.AllOf<DenizenLanguage>().Paginate((lang) => lang.GetListString())
                    .Select((page) => new EmbedBuilder().WithColor(Color.Gold).WithTitle("All known language explanations").WithDescription(page).Build())
                    .ToList();
                await command.ReplyAsync(new DiscordPaginatedMessage(pages));
                return;
            }
            if (searchLang.StartsWith('\\'))
            {
                searchLang = searchLang.Substring(1);
            }
            if (!await command.Bot.HandleSearch<DenizenLanguage>(searchLang, command))
            {
                await command.ReplyAsync(new DiscordEmbedMessage(new EmbedBuilder().WithColor(Color.Red)
                    .WithDescription("No language explanations were found matching the specified input. :pensive:").Build()));
            }
        }

        /// <summary>
        /// Searches all meta documentation.
        /// </summary>
        [CommandHandler("search", "s")]
        public static async Task SearchAll(BotCommand command)
        {
            string search = command.Arguments.Stringify((x) => x, " ").ToLower();
            if (string.IsNullOrWhiteSpace(search))
            {
                return;
            }
            if (search == "all")
            {
                List<Embed> pages = command.Bot.Meta.AllOf<IDenizenMetaType>().Paginate((meta) => meta.GetListString())
                    .Select((page) => new EmbedBuilder().WithColor(Color.Gold).WithTitle("All known meta").WithDescription(page).Build())
                    .ToList();
                await command.ReplyAsync(new DiscordPaginatedMessage(pages));
                return;
            }
            if (search.StartsWith('\\'))
            {
                search = search.Substring(1);
            }
            if (!await command.Bot.HandleSearch<IDenizenMetaType>(search, command))
            {
                await command.ReplyAsync(new DiscordEmbedMessage(new EmbedBuilder().WithColor(Color.Red)
                    .WithDescription("No meta was found matching the specified input. :pensive:").Build()));
            }
        }

        /// <summary>
        /// Reloads all meta.
        /// </summary>
        [CommandHandler("reload")]
        public static async Task ReloadMeta(BotCommand command)
        {
            await command.ReplyAsync(new SimpleMessage("Reloading meta..."));
            Stopwatch sw = new Stopwatch();
            sw.Start();
            command.Bot.Meta.Reload();
            sw.Stop();
            string output = string.Empty;
            foreach (KeyValuePair<string, Type> pair in command.Bot.Meta.KnownMetaTypes)
            {
                output += $" ({pair.Key}: {command.Bot.Meta.AllOf(pair.Value).Count()})";
            }
            await command.ReplyAsync(new SimpleMessage($"Succesfully reloaded meta{output} in {sw.ElapsedMilliseconds / 1000} seconds"));
        }

        internal static string AdaptMatchLevel(SearchMatchLevel matchLevel)
        {
            switch (matchLevel)
            {
                case SearchMatchLevel.PARTIAL:
                    return "Partial match";
                case SearchMatchLevel.SIMILAR:
                    return "Similar match";
                case SearchMatchLevel.VERY_SIMILAR:
                    return "Most likely match";
                case SearchMatchLevel.EXACT:
                    return "Exact match";
                default:
                    throw new InvalidOperationException("Match level could not be adapted: " + matchLevel);
            }
        }
    }
}
