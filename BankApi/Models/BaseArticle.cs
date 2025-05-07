namespace BankApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using BankApi.Validators;

    /// <summary>
    /// Represents the base article model containing common properties for articles.
    /// </summary>
    public class BaseArticle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseArticle"/> class.
        /// </summary>
        /// <param name="articleId">The unique identifier for the article.</param>
        /// <param name="title">The title of the article.</param>
        /// <param name="summary">A brief summary of the article.</param>
        /// <param name="content">The main content of the article.</param>
        /// <param name="publishedOn">The date and time the article was published.</param>
        /// <param name="status">The publication status of the article.</param>
        public BaseArticle(
            int articleId,
            string title,
            string summary,
            string content,
            DateTime publishedOn,
            Status status)
        {
            this.ArticleId = articleId;
            this.Title = title;
            this.Summary = summary;
            this.Content = content;
            this.PublishedOn = publishedOn;
            this.Status = status;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the article.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the title of the article.
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a brief summary of the article.
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the main content of the article.
        /// </summary>
        [MaxLength(256)]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the date and time the article was published.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PublishedOn { get; set; }

        /// <summary>
        /// Gets or sets the publication status of the article.
        /// </summary>
        [Required]
        [StatusValidation]
        public Status Status { get; set; }

        /// <summary>
        /// Gets or sets the list of stocks related to the article.
        /// </summary>
        public List<BaseStock> RelatedStocks { get; set; } = [];
    }
}
