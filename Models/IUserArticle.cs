namespace StockApp.Models
{
    using System;
    using System.Collections.Generic;

    public interface IUserArticle
    {
        string ArticleId { get; set; }

        string Title { get; set; }

        string Summary { get; set; }

        string Content { get; set; }

        string Author { get; set; }

        DateTime SubmissionDate { get; set; }

        string Status { get; set; }

        string Topic { get; set; }

        IReadOnlyList<string> RelatedStocks { get; set; }
    }
}
