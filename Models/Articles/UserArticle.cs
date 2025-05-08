namespace StockApp.Models.Articles
{
    using System;
    using StockApp.Models;

    /// <summary>
    /// Represents an article created by a user.
    /// </summary>
    public class UserArticle : BaseArticle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserArticle"/> class.
        /// </summary>
        public UserArticle()
        {
            Topic = string.Empty;
            Status = Status.Pending;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserArticle"/> class.
        /// </summary>
        /// <param name="title">The title of the article.</param>
        /// <param name="summary">A brief summary of the article.</param>
        /// <param name="content">The content of the article.</param>
        /// <param name="publishedOn">The date and time the article was published.</param>
        /// <param name="authorId">The unique identifier of the author.</param>
        /// <param name="topic">The topic of the article.</param>
        /// <param name="status">The publication status of the article.</param>
        public UserArticle(
            string title,
            string summary,
            string content,
            DateTime publishedOn,
            int authorId,
            string topic,
            Status status = Status.Pending)
            : base(title, summary, content, publishedOn)
        {
            AuthorId = authorId;
            Topic = topic;
            Status = status;
        }

        /// <summary>
        /// Gets or sets the topic of the article.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the publication status of the article.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the author.
        /// </summary>
        public int AuthorId { get; set; }
    }
}
