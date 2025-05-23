using BankApi.Repositories; // Added missing using
using Common.Models;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController(INewsService newsService, IUserRepository userRepository) : ControllerBase
    {
        private readonly INewsService _newsService = newsService;
        private readonly IUserRepository _userRepository = userRepository;

        private async Task<string> GetCurrentUserCnp()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userRepository.GetByIdAsync(int.Parse(userId));
            return user == null ? throw new Exception("User not found") : user.CNP;
        }

        private async Task<User> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userRepository.GetByIdAsync(int.Parse(userId));
            return user ?? throw new Exception("User not found");
        }

        [HttpGet]
        public async Task<ActionResult<List<NewsArticle>>> GetNewsArticles()
        {
            return await _newsService.GetNewsArticlesAsync();
        }

        [HttpGet("{articleId}")]
        public async Task<ActionResult<NewsArticle>> GetNewsArticleById(string articleId)
        {
            return await _newsService.GetNewsArticleByIdAsync(articleId);
        }

        [HttpPost("{articleId}/markasread")]
        public async Task<ActionResult<bool>> MarkArticleAsRead(string articleId)
        {
            return await _newsService.MarkArticleAsReadAsync(articleId);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin,Moderator")] // Assuming admins and moderators can create articles
        public async Task<ActionResult<bool>> CreateArticle([FromBody] NewsArticle article)
        {
            // Author will be set based on the authenticated user or a specific logic in the service
            return await _newsService.CreateArticleAsync(article);
        }

        [HttpGet("userarticles")]
        [Authorize(Roles = "Admin")] // Only admins can see all user articles by CNP
        public async Task<ActionResult<List<NewsArticle>>> GetUserArticles(string authorCnp, Status status = Status.All, string topic = "All")
        {
            return await _newsService.GetUserArticlesAsync(status, topic, authorCnp);
        }

        [HttpPost("{articleId}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> ApproveUserArticle(string articleId)
        {
            var userCnp = await GetCurrentUserCnp();
            return await _newsService.ApproveUserArticleAsync(userCnp, articleId);
        }

        [HttpPost("{articleId}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> RejectUserArticle(string articleId)
        {
            var userCnp = await GetCurrentUserCnp();
            return await _newsService.RejectUserArticleAsync(userCnp, articleId);
        }

        [HttpDelete("{articleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteUserArticle(string articleId)
        {
            var userCnp = await GetCurrentUserCnp();
            return await _newsService.DeleteUserArticleAsync(userCnp, articleId);
        }

        [HttpPost("submit")]
        public async Task<ActionResult<bool>> SubmitUserArticle([FromBody] NewsArticle article)
        {
            var userCnp = await GetCurrentUserCnp();
            // The service should handle setting the author based on the authenticated user
            return await _newsService.SubmitUserArticleAsync(article, userCnp);
        }

        [HttpGet("{articleId}/relatedstocks")]
        public async Task<ActionResult<List<Stock>>> GetRelatedStocksForArticle(string articleId)
        {
            return await _newsService.GetRelatedStocksForArticleAsync(articleId);
        }
    }
}
