namespace BankApi.Repositories.Articles
{
    using BankApi.Data;
    using BankApi.Models;
    using BankApi.Models.Articles;
    using Microsoft.EntityFrameworkCore;

    public class UserArticlesRepository : IUserArticlesRepository
    {
        private readonly ApiDbContext _context;

        public UserArticlesRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserArticle>> GetAllUserArticlesAsync() =>
            await _context.UserArticles.ToListAsync();

        public async Task<List<UserArticle>> GetUserArticlesByStatusAsync(Status status) =>
            await _context.UserArticles.Where(article => article.Status == status).ToListAsync();

        public async Task<List<UserArticle>> GetUserArticlesByTopicAsync(string topic) =>
            await _context.UserArticles.Where(article => article.Topic == topic).ToListAsync();

        public async Task<UserArticle?> GetUserArticleByIdAsync(int articleId) =>
            await _context.UserArticles.FindAsync(articleId);

        public async Task AddUserArticleAsync(UserArticle article)
        {
            await _context.UserArticles.AddAsync(article);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ApproveUserArticleAsync(int articleId)
        {
            var article = await _context.UserArticles.FindAsync(articleId);
            if (article == null)
            {
                return false;
            }

            article.Status = Status.Approved;

            _context.UserArticles.Attach(article);
            _context.Entry(article).Property(a => a.Status).IsModified = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectUserArticleAsync(int articleId)
        {
            var article = await _context.UserArticles.FindAsync(articleId);
            if (article == null)
            {
                return false;
            }

            article.Status = Status.Rejected;

            _context.UserArticles.Attach(article);
            _context.Entry(article).Property(a => a.Status).IsModified = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task UpdateUserArticleAsync(UserArticle userArticle)
        {
            _context.UserArticles.Update(userArticle);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserArticleAsync(int articleId)
        {
            var article = await _context.UserArticles.FindAsync(articleId);
            if (article == null)
            {
                return false;
            }

            _context.UserArticles.Remove(article);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
