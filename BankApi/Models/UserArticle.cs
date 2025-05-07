namespace BankApi.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents an article created by a user.
    /// </summary>
    public class UserArticle : BaseArticle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserArticle"/> class.
        /// </summary>
        /// <param name="articleId">The unique identifier of the article.</param>
        /// <param name="title">The title of the article.</param>
        /// <param name="summary">A brief summary of the article.</param>
        /// <param name="content">The content of the article.</param>
        /// <param name="publishedOn">The date and time the article was published.</param>
        /// <param name="status">The publication status of the article.</param>
        /// <param name="topic">The topic of the article.</param>
        /// <param name="authorId">The unique identifier of the author.</param>
        public UserArticle(
            int articleId,
            string title,
            string summary,
            string content,
            DateTime publishedOn,
            Status status,
            string topic,
            int authorId)
            : base(articleId, title, summary, content, publishedOn, status)
        {
            this.Topic = topic;
            this.AuthorId = authorId;
        }

        /// <summary>
        /// Gets or sets the topic of the article.
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the author.
        /// </summary>
        [ForeignKey(nameof(AuthorId))]
        public int AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the author of the article.
        /// </summary>
        public User Author { get; set; } = new();
    }
}
