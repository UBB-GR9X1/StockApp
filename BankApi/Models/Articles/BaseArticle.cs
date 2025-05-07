namespace BankApi.Models.Articles
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using BankApi.Models;

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
        public BaseArticle(
            int articleId,
            string title,
            string summary,
            string content,
            DateTime publishedOn)
        {
            ArticleId = articleId;
            Title = title;
            Summary = summary;
            Content = content;
            PublishedOn = publishedOn;
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
        /// Gets or sets the list of stocks related to the article.
        /// </summary>
        public List<BaseStock> RelatedStocks { get; set; } = [];
    }
}
