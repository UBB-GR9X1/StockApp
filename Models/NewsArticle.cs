namespace StockApp.Models
{
    using System.Collections.Generic;

    public class NewsArticle
    {
        public string ArticleId { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public string Source { get; set; }

        public string PublishedDate { get; set; }

        public bool IsRead { get; set; }

        public bool IsWatchlistRelated { get; set; }

        public string Category { get; set; }

        public List<string> RelatedStocks { get; set; }

        public Status Status { get; set; } // 0 = Pending, 1 = Approved, 2 = Rejected
    }
}
