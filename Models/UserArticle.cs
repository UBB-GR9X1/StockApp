namespace StockApp.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an article created by a user, including metadata and related stocks.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="UserArticle"/> class with specified details.
    /// </remarks>
    /// <param name="articleId">Unique identifier of the article.</param>
    /// <param name="title">Title of the article.</param>
    /// <param name="summary">Short summary of the article.</param>
    /// <param name="content">Full content of the article.</param>
    /// <param name="author">The user who authored the article.</param>
    /// <param name="submissionDate">Date and time when the article was submitted.</param>
    /// <param name="status">Current status of the article (e.g., "Pending", "Published").</param>
    /// <param name="topic">Topic or category of the article.</param>
    /// <param name="relatedStocks">Collection of related stock symbols or names.</param>
    public class UserArticle(
        string articleId,
        string title,
        string summary,
        string content,
        User author,
        DateTime submissionDate,
        string status,
        string topic,
        IEnumerable<string> relatedStocks)
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
        /// Gets or sets the user who authored the article.
        /// </summary>
        public User Author { get; set; } = author;

        /// <summary>
        /// Gets or sets the date and time when the article was submitted.
        /// </summary>
        public DateTime SubmissionDate { get; set; } = submissionDate;

        /// <summary>
        /// Gets or sets the current status of the article.
        /// </summary>
        public string Status { get; set; } = status;

        /// <summary>
        /// Gets or sets the topic or category of the article.
        /// </summary>
        public string Topic { get; set; } = topic;

        /// <summary>
        /// Gets or sets the list of related stock symbols or names.
        /// </summary>
        public List<string> RelatedStocks { get; set; } = [.. relatedStocks];
    }
}
