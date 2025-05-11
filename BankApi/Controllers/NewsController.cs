namespace BankApi.Controllers
{
    using System;
    using System.Threading.Tasks;
    using BankApi.Models;
    using BankApi.Repositories;
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
        public async Task<IActionResult> AddNewsArticleAsync(NewsArticle newsArticle)
        {
            try
            {
                await _repository.AddNewsArticleAsync(newsArticle);
                return Ok("News article added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding news article.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("UpdateNewsArticle")]
        public async Task<IActionResult> UpdateNewsArticleAsync(NewsArticle newsArticle)
        {
            try
            {
                await _repository.UpdateNewsArticleAsync(newsArticle);
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
        public async Task<IActionResult> GetNewsArticleByIdAsync(string articleId)
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
        public async Task<IActionResult> GetAllNewsArticlesAsync()
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
        public async Task<IActionResult> GetNewsArticlesByAuthorCNPAsync(string authorCNP)
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
        public async Task<IActionResult> GetNewsArticlesByCategoryAsync(string category)
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
        public async Task<IActionResult> GetNewsArticlesByStockAsync(string stockName)
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
        public async Task<IActionResult> MarkArticleAsReadAsync(string articleId)
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
    }
}