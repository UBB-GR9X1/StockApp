namespace BankApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BankApi.Models.Articles;
    using BankApi.Repositories.Articles;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("api/[controller]")]
    public class NewsArticlesController : ControllerBase
    {
        private readonly INewsArticleRepository _articlesRepository;
        private readonly ILogger<NewsArticlesController> _logger;

        public NewsArticlesController(INewsArticleRepository articlesRepository, ILogger<NewsArticlesController> logger)
        {
            _articlesRepository = articlesRepository ?? throw new ArgumentNullException(nameof(articlesRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/NewsArticles
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<NewsArticle>>> GetAllNewsArticles()
        {
            try
            {
                var articles = await _articlesRepository.GetAllNewsArticlesAsync();
                if (articles.Count == 0)
                {
                    return NoContent();
                }

                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all news articles");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving news articles.");
            }
        }

        // GET: api/NewsArticles/category/{category}
        [HttpGet("category/{category}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<NewsArticle>>> GetNewsArticlesByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return BadRequest("Category is required.");
            }

            try
            {
                var articles = await _articlesRepository.GetNewsArticlesByCategoryAsync(category);
                if (articles.Count == 0)
                {
                    return NoContent();
                }

                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving news articles of category '{Category}'", category);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving news articles.");
            }
        }

        // GET: api/NewsArticles/stock/{stockName}
        [HttpGet("stock/{stockName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<NewsArticle>>> GetNewsArticlesByStock(string stockName)
        {
            if (string.IsNullOrWhiteSpace(stockName))
            {
                return BadRequest("Stock name is required");
            }

            try
            {
                var articles = await _articlesRepository.GetNewsArticlesByStockAsync(stockName);
                if (articles.Count == 0)
                {
                    return NoContent();
                }

                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving news articles about {StockName}'s stocks", stockName);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving news articles.");
            }
        }

        // GET: api/NewsArticles/{articleId}
        [HttpGet("{articleId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<NewsArticle>> GetNewsArticleById(int articleId)
        {
            try
            {
                var article = await _articlesRepository.GetNewsArticleByIdAsync(articleId);
                if (article == null)
                {
                    return NotFound($"Article with ID {articleId} doesn't exist");
                }

                return Ok(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving news article with ID {ArticleId}", articleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving news articles.");
            }
        }

        // POST: api/NewsArticles
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateNewsArticle([FromBody] NewsArticle article)
        {
            if (article == null || string.IsNullOrWhiteSpace(article.Title))
            {
                return BadRequest("Invalid article data.");
            }

            try
            {
                await _articlesRepository.AddNewsArticleAsync(article);
                return CreatedAtAction(nameof(GetNewsArticleById), new { articleId = article.Id }, article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating a new news article");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving the article.");
            }
        }

        // POST: api/NewsArticles/{articleId}/stocks
        [HttpPost("{articleId}/stocks")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddRelatedStocks(int articleId, [FromBody] List<int> stockIds)
        {
            if (stockIds == null || stockIds.Count == 0)
            {
                return BadRequest("Invalid stock IDs.");
            }

            try
            {
                await _articlesRepository.AddRelatedStocksAsync(articleId, stockIds);
                return CreatedAtAction(nameof(GetNewsArticleById), new { articleId }, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding related stocks to article ID {ArticleId}", articleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding related stocks.");
            }
        }

        // PUT: api/NewsArticles/{articleId}
        [HttpPut("{articleId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateNewsArticle(int articleId, [FromBody] NewsArticle newsArticle)
        {
            if (newsArticle == null || articleId != newsArticle.Id)
            {
                return BadRequest("Invalid article data.");
            }

            try
            {
                var existingArticle = await _articlesRepository.GetNewsArticleByIdAsync(articleId);
                if (existingArticle == null)
                {
                    return NotFound($"No article found with ID {articleId}.");
                }

                await _articlesRepository.UpdateNewsArticleAsync(newsArticle);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news article with ID {ArticleId}", articleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the article.");
            }
        }

        // DELETE: api/NewsArticles/{articleId}
        [HttpDelete("{articleId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteNewsArticle(int articleId)
        {
            try
            {
                var deletedSuccessfully = await _articlesRepository.DeleteNewsArticleAsync(articleId);

                if (!deletedSuccessfully)
                {
                    return NotFound($"No article found with ID {articleId}.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting news article with ID {ArticleId}", articleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the article.");
            }
        }

        // PUT: api/NewsArticles/{articleId}/read
        [HttpPut("{articleId}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> MarkNewsArticleAsRead(int articleId)
        {
            if (articleId <= 0)
            {
                return BadRequest("Invalid article ID.");
            }

            try
            {
                var markedAsRead = await _articlesRepository.MarkNewsArticleAsReadAsync(articleId);

                if (!markedAsRead)
                {
                    return NotFound($"No article found with ID {articleId}.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking news article with ID {ArticleId} as read", articleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the article.");
            }
        }
    }
}
