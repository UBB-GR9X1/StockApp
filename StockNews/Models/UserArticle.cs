using System;
using System.Collections.Generic;

namespace StockNewsPage.Models
{
    public class UserArticle
    {
        public string ArticleId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public string Topic { get; set; }
        public List<string> RelatedStocks { get; set; }
    }
}
