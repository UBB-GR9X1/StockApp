namespace BankApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using BankApi.Models;
    using BankApi.Models.Articles;
    using BankApi.Repositories.Articles;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class UserArticlesController : ControllerBase
    {
        private readonly IUserArticlesRepository _articlesRepository;
        private readonly ILogger<UserArticlesController> _logger;

        public UserArticlesController(IUserArticlesRepository userArticlesRepository, ILogger<UserArticlesController> logger)
        {
            _articlesRepository = userArticlesRepository;
            _logger = logger;
        }

        // GET: api/UserArticles
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserArticle>>> GetAllUserArticles()
        {
            try
            {
                var articles = await _articlesRepository.GetAllUserArticlesAsync();
                if (articles.Count == 0)
                {
                    return NotFound();
                }

                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user articles.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving user articles.");
            }
        }

        // GET: api/UserArticles/status/{status}
        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserArticle>>> GetUserArticlesByStatus(string status)
        {
            Status actualStatus;
            if (status == "Pending")
            {
                actualStatus = Status.Pending;
            }
            else if (status == "Rejected")
            {
                actualStatus = Status.Rejected;
            }
            else if (status == "Accepted")
            {
                actualStatus = Status.Approved;
            }
            else if (status == "All")
            {
                actualStatus = Status.All;
            }
            else
            {
                return BadRequest($"Status '{status}' is invalid");
            }

            try
            {
                var articles = await _articlesRepository.GetUserArticlesByStatusAsync(actualStatus);
                if (articles.Count == 0)
                {
                    return NoContent();
                }

                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user articles by status '{Status}'", actualStatus.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving user articles.");
            }
        }

        // GET: api/UserArticles/topic/{topic}
        [HttpGet("topic/{topic}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserArticle>>> GetUserArticlesByTopic(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                return BadRequest("Topic is required.");
            }

            try
            {
                var articles = await _articlesRepository.GetUserArticlesByTopicAsync(topic);
                if (articles.Count == 0)
                {
                    return NoContent();
                }

                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user articles by topic '{Topic}'", topic);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving user articles.");
            }
        }

        // GET: api/UserArticles/{articleId}
        [HttpGet("{articleId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserArticle>> GetUserArticleById(int articleId)
        {
            try
            {
                var article = await _articlesRepository.GetUserArticleByIdAsync(articleId);
                if (article == null)
                {
                    return NotFound($"No article found with ID {articleId}.");
                }

                return Ok(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user article with ID {ArticleId}", articleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the article.");
            }
        }

        // POST: api/UserArticles
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddUserArticle([FromBody] UserArticle article)
        {
            if (article == null || string.IsNullOrWhiteSpace(article.Title))
            {
                return BadRequest("Invalid article data.");
            }

            try
            {
                await _articlesRepository.AddUserArticleAsync(article);
                return CreatedAtAction(nameof(GetUserArticleById), new { articleId = article.Id }, article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating a new user article");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving the article.");
            }
        }

        // PUT: api/UserArticles/{articleId}/approve
        [HttpPut("{articleId}/approve")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApproveUserArticle(int articleId)
        {
            try
            {
                var approved = await _articlesRepository.ApproveUserArticleAsync(articleId);
                if (!approved)
                {
                    return NotFound($"No article found with ID {articleId}.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving user article with ID {ArticleId}", articleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while approving the article.");
            }
        }

        // PUT: api/UserArticles/{articleId}/reject
        [HttpPut("{articleId}/reject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RejectUserArticle(int articleId)
        {
            try
            {
                var rejected = await _articlesRepository.RejectUserArticleAsync(articleId);
                if (!rejected)
                {
                    return NotFound($"No article found with ID {articleId}.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user article with ID {ArticleId}", articleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while rejecting the article.");
            }
        }

        // PUT: api/UserArticles/{articleId}
        [HttpPut("{articleId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUserArticle(int articleId, [FromBody] UserArticle userArticle)
        {
            if (userArticle == null || articleId != userArticle.Id || string.IsNullOrWhiteSpace(userArticle.Title))
            {
                return BadRequest("Invalid article data.");
            }

            try
            {
                var existingArticle = await _articlesRepository.GetUserArticleByIdAsync(articleId);
                if (existingArticle == null)
                {
                    return NotFound($"No article found with ID {articleId}.");
                }

                await _articlesRepository.UpdateUserArticleAsync(userArticle);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user article with ID {ArticleId}", articleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the article.");
            }
        }

        // DELETE: api/UserArticles/{articleId}
        [HttpDelete("{articleId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUserArticle(int articleId)
        {
            try
            {
                var deleted = await _articlesRepository.DeleteUserArticleAsync(articleId);
                if (!deleted)
                {
                    return NotFound($"No article found with ID {articleId}.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user article with ID {ArticleId}", articleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the article.");
            }
        }
    }
}
