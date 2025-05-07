namespace StockApp.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a news article about stocks, including metadata and content.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="NewsArticle"/> class with specified details.
    /// </remarks>
    /// <param name="articleId">Unique identifier of the article.</param>
    /// <param name="title">Title of the article.</param>
    /// <param name="summary">Short summary of the article.</param>
    /// <param name="content">Full content of the article.</param>
    /// <param name="source">Source from which the article was retrieved.</param>
    /// <param name="publishedDate">Publication date of the article.</param>
    /// <param name="relatedStocks">Collection of related stock symbols or names.</param>
    /// <param name="status">Current status of the article (e.g., Pending, Published).</param>
    public class NewsArticle(
        string articleId,
        string title,
        string summary,
        string content,
        string source,
        DateTime publishedDate,
        IEnumerable<string> relatedStocks,
        Status status = Status.Pending)
    {
        /// <summary>
        /// Gets or sets the unique identifier of the article.
        /// </summary>
        public string ArticleId { get; set; } = articleId;

        /// <summary>
        /// Gets or sets the title of the article.
        /// </summary>
        public string Title { get; set; } = title;

        /// <summary>
        /// Gets or sets a short summary of the article.
        /// </summary>
        public string Summary { get; set; } = summary;

        /// <summary>
        /// Gets or sets the full content of the article.
        /// </summary>
        public string Content { get; set; } = content;

        /// <summary>
        /// Gets or sets the source of the article.
        /// </summary>
        public string Source { get; set; } = source;

        /// <summary>
        /// Gets or sets the publication date of the article.
        /// </summary>
        public DateTime PublishedDate { get; set; } = publishedDate;

        /// <summary>
        /// Gets or sets a value indicating whether the article has been read.
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this article relates to the watchlist.
        /// </summary>
        public bool IsWatchlistRelated { get; set; }

        /// <summary>
        /// Gets or sets the category of the article.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of related stock symbols or names.
        /// </summary>
        public List<string> RelatedStocks { get; set; } = [.. relatedStocks];

        /// <summary>
        /// Gets or sets the current status of the article.
        /// </summary>
        public Status Status { get; set; } = status;
    }
}
