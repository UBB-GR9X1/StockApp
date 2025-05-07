namespace BankApi.Models
{
    using BankApi.Models.Articles;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class NewsArticleStock
    {
        public NewsArticleStock(int newsArticleId, int stockId)
        {
            ArticleId = newsArticleId;
            StockId = stockId;
        }

        [Required]
        public int ArticleId { get; set; }

        [ForeignKey(nameof(ArticleId))]
        public NewsArticle Article { get; set; } = new();

        [Required]
        public int StockId { get; set; }

        [ForeignKey(nameof(StockId))]
        public BaseStock Stock { get; set; } = new();

        public List<NewsArticle> RelatedNewsArticles = [];

        public List<BaseStock> RelatedStocks = [];
    }
}
