using Discord;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Integrations.Chat.Discord
{
    /// <summary>
    /// Represents a paginated Discord embed message.
    /// </summary>
    public class DiscordPaginatedMessage : IMessage
    {
        /// <summary>
        /// The pages.
        /// </summary>
        public List<Embed> Pages;

        /// <summary>
        /// The number of pages.
        /// </summary>
        public int PageCount;

        /// <summary>
        /// The current page.
        /// </summary>
        public int CurrentPage;

        /// <summary>
        /// The message to edit.
        /// </summary>
        public RestUserMessage MessageToEdit;

        /// <summary>
        /// Constructs a new paginated Discord embed message.
        /// </summary>
        /// <param name="pages">The pages.</param>
        public DiscordPaginatedMessage(List<Embed> pages)
        {
            Pages = pages;
            PageCount = pages.Count;
            CurrentPage = 0;
        }

        /// <summary>
        /// Gets the specified page as an embed with a page number footer.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <returns>A new Discord embed.</returns>
        public Embed GetPage(int page)
        {
            return Pages[page].ToEmbedBuilder().WithFooter($"Page {page + 1} / {PageCount}").Build();
        }
    }
}
