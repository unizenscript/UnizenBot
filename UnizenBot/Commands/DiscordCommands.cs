using Discord.Rest;
using UnizenBot.Integrations.Chat;
using UnizenBot.Integrations.Chat.Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnizenBot.Commands
{
    /// <summary>
    /// Contains general Discord commands.
    /// </summary>
    public class DiscordCommands
    {
        /// <summary>
        /// Moves through <see cref="DiscordPaginatedMessage"/> pages.
        /// </summary>
        [CommandHandler("page", "p")]
        public static async Task PageCommand(BotCommand command)
        {
            DiscordUserMessage message = command.Message as DiscordUserMessage;
            if (message == null)
            {
                return;
            }
            if (message.Discord.LastPageError.Remove(message.DiscordMessage.Channel.Id, out RestUserMessage error))
            {
                await error.DeleteAsync();
            }
            if (command.Bot.DiscordConnection.LastPaginated.TryGetValue(message.DiscordMessage.Channel.Id, out DiscordPaginatedMessage paginated))
            {
                if (command.Arguments.Length > 0)
                {
                    string arg = command.Arguments[0].ToLower();
                    if (arg[0] == 'n') // next
                    {
                        if (paginated.CurrentPage < paginated.PageCount - 1)
                        {
                            paginated.CurrentPage++;
                            await paginated.MessageToEdit.ModifyAsync((x) => x.Embed = paginated.GetPage(paginated.CurrentPage));
                        }
                    }
                    else if (arg[0] == 'p') // previous
                    {
                        if (paginated.CurrentPage > 0)
                        {
                            paginated.CurrentPage--;
                            await paginated.MessageToEdit.ModifyAsync((x) => x.Embed = paginated.GetPage(paginated.CurrentPage));
                        }
                    }
                    else if (arg[0] == 'l') // last
                    {
                        if (paginated.CurrentPage < paginated.PageCount - 1)
                        {
                            paginated.CurrentPage = paginated.PageCount - 1;
                            await paginated.MessageToEdit.ModifyAsync((x) => x.Embed = paginated.GetPage(paginated.CurrentPage));
                        }
                    }
                    else if (int.TryParse(arg, out int num))
                    {
                        if (num > paginated.PageCount)
                        {
                            num = paginated.PageCount;
                        }
                        num--;
                        if (num < 0)
                        {
                            num = 0;
                        }
                        if (paginated.CurrentPage != num)
                        {
                            paginated.CurrentPage = num;
                            await paginated.MessageToEdit.ModifyAsync((x) => x.Embed = paginated.GetPage(paginated.CurrentPage));
                        }
                    }
                    else
                    {
                        message.Discord.LastPageError[message.DiscordMessage.Channel.Id] = await message.DiscordMessage.Channel
                            .SendMessageAsync($"Invalid command '!{command.Alias} {command.Arguments[0]}' Syntax: !{command.Alias} #/next/prev/last");
                    }
                }
                else
                {
                    message.Discord.LastPageError[message.DiscordMessage.Channel.Id] = await message.DiscordMessage.Channel
                        .SendMessageAsync($"Invalid command '!{command.Alias}' Syntax: !{command.Alias} #/next/prev/last");
                }
                await message.DiscordMessage.DeleteAsync();
            }
            else
            {
                message.Discord.LastPageError[message.DiscordMessage.Channel.Id] = await message.DiscordMessage.Channel.SendMessageAsync("No recent paginated commands have been executed.");
            }
        }
    }
}
