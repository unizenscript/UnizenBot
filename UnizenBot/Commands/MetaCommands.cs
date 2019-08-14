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
            await command.Bot.HandleSearch<DenizenCommand>(command.Arguments[0], command);
        }

        /// <summary>
        /// Searches documented tags.
        /// </summary>
        [CommandHandler("tag", "t")]
        public static async Task SearchTags(BotCommand command)
        {
            await command.Bot.HandleSearch<DenizenTag>(command.Arguments[0], command);
        }

        /// <summary>
        /// Searches documented mechanisms.
        /// </summary>
        [CommandHandler("mechanism", "mech", "mec", "m")]
        public static async Task SearchMechanisms(BotCommand command)
        {
            await command.Bot.HandleSearch<DenizenMechanism>(command.Arguments[0], command);
        }

        /// <summary>
        /// Searches documented events.
        /// </summary>
        [CommandHandler("event", "evt", "e")]
        public static async Task SearchEvents(BotCommand command)
        {
            await command.Bot.HandleSearch<DenizenEvent>(command.Arguments.Stringify((x) => x, " "), command);
        }

        /// <summary>
        /// Searches documented events.
        /// </summary>
        [CommandHandler("action", "act", "a")]
        public static async Task SearchActions(BotCommand command)
        {
            await command.Bot.HandleSearch<DenizenAction>(command.Arguments.Stringify((x) => x, " "), command);
        }

        /// <summary>
        /// Searches documented language explanations.
        /// </summary>
        [CommandHandler("language", "lang", "lng", "l")]
        public static async Task SearchLanguages(BotCommand command)
        {
            await command.Bot.HandleSearch<DenizenLanguage>(command.Arguments.Stringify((x) => x, " "), command);
        }

        /// <summary>
        /// Searches all meta documentation.
        /// </summary>
        [CommandHandler("search", "s")]
        public static async Task GeneralSearch(BotCommand command)
        {
            await command.Bot.HandleSearch<IDenizenMetaType>(command.Arguments.Stringify((x) => x, " "), command);
        }

        /// <summary>
        /// Searches all meta documentation and never returns an exact result.
        /// </summary>
        [CommandHandler("searchall", "sa")]
        public static async Task GeneralSearchAll(BotCommand command)
        {
            await command.Bot.HandleSearch<IDenizenMetaType>(command.Arguments.Stringify((x) => x, " "), command, true);
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
                case SearchMatchLevel.DID_YOU_MEAN:
                    return "Did you mean";
                case SearchMatchLevel.BACKUP:
                    return "Backup match";
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

        internal static string AdaptMatchLevelPlural(SearchMatchLevel matchLevel)
        {
            switch (matchLevel)
            {
                case SearchMatchLevel.DID_YOU_MEAN:
                    return "Did you mean";
                case SearchMatchLevel.BACKUP:
                    return "Backup matches";
                case SearchMatchLevel.PARTIAL:
                    return "Partial matches";
                case SearchMatchLevel.SIMILAR:
                    return "Similar matches";
                case SearchMatchLevel.VERY_SIMILAR:
                    return "Most likely matches";
                case SearchMatchLevel.EXACT:
                    return "Exact matches";
                default:
                    throw new InvalidOperationException("Match level could not be adapted: " + matchLevel);
            }
        }
    }
}
