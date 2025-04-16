namespace StockApp.Models
{
    using System;
    using System.Collections.Generic;

    public class UserArticle(
        string articleId,
        string title,
        string summary,
        string content,
        string author,
        DateTime submissionDate,
        string status,
        string topic,
        List<string> relatedStocks)
    {
        public string ArticleId { get; set; } = articleId;

        public string Title { get; set; } = title;

        public string Summary { get; set; } = summary;

        public string Content { get; set; } = content;

        public string Author { get; set; } = author;

        public DateTime SubmissionDate { get; set; } = submissionDate;

        public string Status { get; set; } = status;

        public string Topic { get; set; } = topic;

        public List<string> RelatedStocks { get; set; } = relatedStocks;
    }
}
