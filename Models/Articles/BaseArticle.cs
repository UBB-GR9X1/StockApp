namespace StockApp.Models.Articles
{
    using System;
    using System.Collections.Generic;

    /// <summary>  
    /// Represents the base article model containing common properties for articles.  
    /// </summary>  
    public class BaseArticle
    {
        /// <summary>  
        /// Initializes a new instance of the <see cref="BaseArticle"/> class.  
        /// </summary>  
        public BaseArticle()
        {
            Title = string.Empty;
            Summary = string.Empty;
            Content = string.Empty;
        }

        /// <summary>  
        /// Initializes a new instance of the <see cref="BaseArticle"/> class.  
        /// </summary>  
        /// <param name="title">The title of the article.</param>  
        /// <param name="summary">A brief summary of the article.</param>  
        /// <param name="content">The main content of the article.</param>  
        /// <param name="publishedOn">The date and time the article was published.</param>  
        public BaseArticle(
            string title,
            string summary,
            string content,
            DateTime publishedOn)
        {
            Title = title;
            Summary = summary;
            Content = content;
            PublishedOn = publishedOn;
        }

        /// <summary>  
        /// Gets or sets the unique identifier for the article.  
        /// </summary>  
        public int Id { get; set; } = 0;

        /// <summary>  
        /// Gets or sets the title of the article.  
        /// </summary>  
        public string Title { get; set; }

        /// <summary>  
        /// Gets or sets a brief summary of the article.  
        /// </summary>  
        public string Summary { get; set; }

        /// <summary>  
        /// Gets or sets the main content of the article.  
        /// </summary>  
        public string Content { get; set; }

        /// <summary>  
        /// Gets or sets the date and time the article was published.  
        /// </summary>  
        public DateTime PublishedOn { get; set; }
    }
}
