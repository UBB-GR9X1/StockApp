namespace BankApi.Controllers
{
    using System;
    using System.Threading.Tasks;
    using BankApi.Repositories;
    using Common.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Controller for managing news-related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsRepository _repository;
        private readonly ILogger<NewsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsController"/> class.
        /// </summary>
        /// <param name="repository">The news repository.</param>
        /// <param name="logger">The logger.</param>
        public NewsController(INewsRepository repository, ILogger<NewsController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("AddNewsArticle")]
        public async Task<IActionResult> AddNewsArticleAsync([FromBody] NewsArticleApi newsArticle)
        {
            try
            {
                NewsArticle actualArticle = new()
                {
                    ArticleId = newsArticle.ArticleId,
                    Title = newsArticle.Title,
                    Summary = newsArticle.Summary,
                    Content = newsArticle.Content,
                    Source = newsArticle.Source,
                    Topic = newsArticle.Topic,
                    PublishedDate = newsArticle.PublishedDate,
                    RelatedStocks = [.. newsArticle.RelatedStocks.Select(rs => new Stock
                    {
                        Name = rs.Name,
                        Symbol = rs.Symbol,
                        Price = (int)rs.Price,
                        Quantity = rs.Quantity
                    })],
                    Status = newsArticle.Status,
                    Category = newsArticle.Category,
                    Author = newsArticle.Author,
                    AuthorCNP = newsArticle.Author.CNP,
                    IsRead = newsArticle.IsRead
                };
                await _repository.AddNewsArticleAsync(actualArticle);
                return Ok("News article added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding news article.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("UpdateNewsArticle")]
        public async Task<IActionResult> UpdateNewsArticleAsync([FromBody] NewsArticleApi newsArticle)
        {
            try
            {
                NewsArticle updatatedArticle = new()
                {
                    ArticleId = newsArticle.ArticleId,
                    Title = newsArticle.Title,
                    Summary = newsArticle.Summary,
                    Content = newsArticle.Content,
                    Source = newsArticle.Source,
                    Topic = newsArticle.Topic,
                    PublishedDate = newsArticle.PublishedDate,
                    RelatedStocks = [.. newsArticle.RelatedStocks.Select(rs => new Stock
                    {
                        Name = rs.Name,
                        Symbol = rs.Symbol,
                        Price = (int)rs.Price,
                        Quantity = rs.Quantity
                    })],
                    Status = newsArticle.Status,
                    Category = newsArticle.Category,
                    Author = newsArticle.Author,
                    AuthorCNP = newsArticle.Author.CNP,
                    IsRead = newsArticle.IsRead
                };
                await _repository.UpdateNewsArticleAsync(updatatedArticle);
                return Ok("News article updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news article.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("DeleteNewsArticle")]
        public async Task<IActionResult> DeleteNewsArticleAsync(string articleId)
        {
            try
            {
                await _repository.DeleteNewsArticleAsync(articleId);
                return Ok("News article deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting news article.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetNewsArticleById")]
        public async Task<ActionResult<NewsArticle>> GetNewsArticleByIdAsync(string articleId)
        {
            try
            {
                var article = await _repository.GetNewsArticleByIdAsync(articleId);
                return Ok(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving news article by ID.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetAllNewsArticles")]
        public async Task<ActionResult<List<NewsArticle>>> GetAllNewsArticlesAsync()
        {
            try
            {
                var articles = await _repository.GetAllNewsArticlesAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all news articles.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetNewsArticlesByAuthorCNP")]
        public async Task<ActionResult<List<NewsArticle>>> GetNewsArticlesByAuthorCNPAsync(string authorCNP)
        {
            try
            {
                var articles = await _repository.GetNewsArticlesByAuthorCNPAsync(authorCNP);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving news articles by author CNP.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetNewsArticlesByCategory")]
        public async Task<ActionResult<List<NewsArticle>>> GetNewsArticlesByCategoryAsync(string category)
        {
            try
            {
                var articles = await _repository.GetNewsArticlesByCategoryAsync(category);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving news articles by category.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetNewsArticlesByStock")]
        public async Task<ActionResult<List<NewsArticle>>> GetNewsArticlesByStockAsync(string stockName)
        {
            try
            {
                var articles = await _repository.GetNewsArticlesByStockAsync(stockName);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving news articles by stock.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("MarkArticleAsRead")]
        public async Task<IActionResult> MarkArticleAsReadAsync([FromBody] string articleId)
        {
            try
            {
                await _repository.MarkArticleAsReadAsync(articleId);
                return Ok("Article marked as read successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking article as read.");
                return StatusCode(500, "Internal server error.");
            }
        }

        public class NewsArticleApi
        {
            public string ArticleId { get; set; }
            public string Title { get; set; }
            public string Summary { get; set; }
            public string Content { get; set; }
            public string Source { get; set; }
            public DateTime PublishedDate { get; set; }
            public bool IsRead { get; set; }
            public bool IsWatchlistRelated { get; set; }
            public string Category { get; set; }
            public List<RelatedStock> RelatedStocks { get; set; }
            public Status Status { get; set; }
            public string Topic { get; set; }
            public User Author { get; set; }

            public class RelatedStock
            {
                public decimal Price { get; set; }
                public int Quantity { get; set; }
                public string Name { get; set; }
                public string Symbol { get; set; }
                public string AuthorCNP { get; set; }
            }
        }
    }
}