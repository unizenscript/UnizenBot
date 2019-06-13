using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using UnizenBot.Integrations.Chat.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnizenBot.Integrations.Chat.Discord
{
    /// <summary>
    /// Represents a message from a user on Discord.
    /// </summary>
    public class DiscordUserMessage : AbstractSimpleUserMessage
    {
        /// <summary>
        /// The current Discord connection.
        /// </summary>
        public DiscordConnection Discord;

        /// <summary>
        /// The internal Discord message.
        /// </summary>
        public SocketUserMessage DiscordMessage;

        /// <summary>
        /// Creates a new message based on a Discord message.
        /// </summary>
        /// <param name="discord">The current Discord connection.</param>
        /// <param name="message">The Discord message.</param>
        public DiscordUserMessage(DiscordConnection discord, SocketUserMessage message)
        {
            Discord = discord;
            DiscordMessage = message;
        }

        /// <summary>
        /// Whether this message mentions the bot.
        /// </summary>
        public override bool HasMentionPrefix(out int argPos)
        {
            argPos = 0;
            return DiscordMessage.HasMentionPrefix(Discord.Client.CurrentUser, ref argPos);
        }

        /// <summary>
        /// Retrieves the most basic version of this message, compatible with any text-based platforms.
        /// </summary>
        public override string GetSimpleText()
        {
            return DiscordMessage.Content;
        }

        private static readonly IEmote[] Reactions = new IEmote[]
        {
            DiscordConnection.FirstPage, DiscordConnection.PreviousPage, DiscordConnection.NextPage, DiscordConnection.LastPage
        };

        /// <summary>
        /// Replies to the user message without a mention.
        /// </summary>
        /// <param name="message">The message to reply with.</param>
        public override async Task ReplyAsync(IMessage message)
        {
            try
            {
                switch (message)
                {
                    case DiscordEmbedMessage embed:
                        await DiscordMessage.Channel.SendMessageAsync(embed: embed.Internal);
                        break;
                    case DiscordPaginatedMessage paginated:
                        RestUserMessage edit = await DiscordMessage.Channel.SendMessageAsync(embed: paginated.GetPage(0));
                        if (paginated.PageCount > 1)
                        {
                            paginated.MessageToEdit = edit;
                            _ = paginated.MessageToEdit.AddReactionsAsync(Reactions);
                            Discord.PaginatedMessages.Add(paginated.MessageToEdit.Id, paginated);
                            Discord.LastPaginated[DiscordMessage.Channel.Id] = paginated;
                        }
                        break;
                    case DenizenMetaMessage meta:
                        await ReplyAsync(Discord.GetPaginatedMeta(meta));
                        break;
                    case SimpleMessage simple:
                        await DiscordMessage.Channel.SendMessageAsync(simple.Text);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while responding to Discord user message: " + e.Message);
                Console.WriteLine($"   while responding to: [#{DiscordMessage.Channel.Name}] {DiscordMessage.Author.Username}#{DiscordMessage.Author.Discriminator} {DiscordMessage.Content}");
            }
        }
    }
}
