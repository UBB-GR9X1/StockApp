namespace StockApp.Models
{
    using System.Collections.Generic;

    public class NewsArticle(
        string articleId,
        string title,
        string summary,
        string content,
        string source,
        string publishedDate,
        bool isRead,
        bool isWatchlistRelated,
        string category,
        List<string> relatedStocks,
        Status status)
    {
        public string ArticleId { get; set; } = articleId;

        public string Title { get; set; } = title;

        public string Summary { get; set; } = summary;

        public string Content { get; set; } = content;

        public string Source { get; set; } = source;

        public string PublishedDate { get; set; } = publishedDate;

        public bool IsRead { get; set; } = isRead;

        public bool IsWatchlistRelated { get; set; } = isWatchlistRelated;

        public string Category { get; set; } = category;

        public List<string> RelatedStocks { get; set; } = relatedStocks;

        public Status Status { get; set; } = status; // 0 = Pending, 1 = Approved, 2 = Rejected
    }
}
