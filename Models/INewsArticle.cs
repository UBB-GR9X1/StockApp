namespace StockApp.Models
{
    using System;
    using System.Collections.Generic;

    public interface INewsArticle
    {
        string ArticleId { get; set; }

        string Title { get; set; }

        string Summary { get; set; }

        string Content { get; set; }

        string Source { get; set; }

        string PublishedDate { get; set; }

        bool IsRead { get; set; }

        bool IsWatchlistRelated { get; set; }

        string Category { get; set; }

        IReadOnlyList<string> RelatedStocks { get; set; }
        Status Status { get; set; }
    }
}
