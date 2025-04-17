namespace StockApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NewsArticle : INewsArticle
    {
        public string ArticleId { get; set;  }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public string Source { get; set; }

        public string PublishedDate { get; set; }

        public bool IsRead { get; set; }

        public bool IsWatchlistRelated { get; set; }

        public string Category { get; set; }

        public IReadOnlyList<string> RelatedStocks { get; set; }

        public Status Status { get; set; }

        public NewsArticle() { }

        public NewsArticle(
            string articleId,
            string title,
            string summary,
            string content,
            string source,
            string publishedDate,
            IEnumerable<string> relatedStocks,
            Status status = Status.Pending)
        {
            ArticleId = articleId;
            Title = title;
            Summary = summary;
            Content = content;
            Source = source;
            PublishedDate = publishedDate;
            RelatedStocks = relatedStocks.ToList();
            Status = status;
        }
    }
}
