namespace BankApi.Models.Articles
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using BankApi.Models;

    /// <summary>
    /// Represents a news article with additional metadata such as source, read status, and category.
    /// </summary>
    public class NewsArticle : BaseArticle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewsArticle"/> class.
        /// </summary>
        /// <param name="articleId">The unique identifier of the article.</param>
        /// <param name="title">The title of the article.</param>
        /// <param name="summary">A brief summary of the article.</param>
        /// <param name="content">The full content of the article.</param>
        /// <param name="publishedOn">The publication date of the article.</param>
        /// <param name="source">The source of the article.</param>
        public NewsArticle(
            int articleId,
            string title,
            string summary,
            string content,
            DateTime publishedOn,
            string source)
            : base(articleId, title, summary, content, publishedOn)
        {
            Source = source;
        }

        /// <summary>
        /// Gets or sets the source of the news article.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the article has been read.
        /// </summary>
        [Required]
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the article is related to the watchlist.
        /// </summary>
        [Required]
        public bool IsWatchlistRelated { get; set; } = false;

        /// <summary>
        /// Gets or sets the category of the article.
        /// </summary>
        [MaxLength(256)]
        public string? Category { get; set; }
    }
}
