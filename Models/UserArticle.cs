namespace StockApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserArticle
    {
        public string ArticleId { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public User Author { get; set; }

        public DateTime SubmissionDate { get; set; }

        public string Status { get; set; }

        public string Topic { get; set; }

        public List<string> RelatedStocks { get; set; }

        public UserArticle() { }

        public UserArticle(
            string articleId,
            string title,
            string summary,
            string content,
            User author,
            DateTime submissionDate,
            string status,
            string topic,
            IEnumerable<string> relatedStocks)
        {
            ArticleId = articleId;
            Title = title;
            Summary = summary;
            Content = content;
            Author = author;
            SubmissionDate = submissionDate;
            Status = status;
            Topic = topic;
            RelatedStocks = relatedStocks.ToList();
        }
    }
}
