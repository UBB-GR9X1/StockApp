namespace BankApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BankApi.Data;
    using BankApi.Models;
    using Microsoft.EntityFrameworkCore;

    public class NewsRepository : INewsRepository
    {
        private readonly ApiDbContext _dbContext;

        public NewsRepository(ApiDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task AddNewsArticleAsync(NewsArticle newsArticle)
        {
            try
            {
                var existingArticle = await this._dbContext.NewsArticles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.ArticleId == newsArticle.ArticleId);

                if (existingArticle != null)
                {
                    await this.UpdateNewsArticleAsync(newsArticle);
                    return;
                }

                await this._dbContext.NewsArticles.AddAsync(newsArticle);
                await this._dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding news article.", ex);
            }
        }

        public async Task UpdateNewsArticleAsync(NewsArticle newsArticle)
        {
            try
            {
                var existingArticle = await this._dbContext.NewsArticles
                    .FirstOrDefaultAsync(a => a.ArticleId == newsArticle.ArticleId);

                if (existingArticle == null)
                {
                    throw new KeyNotFoundException($"Article with ID {newsArticle.ArticleId} not found.");
                }

                this._dbContext.Entry(existingArticle).CurrentValues.SetValues(newsArticle);
                await this._dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating news article.", ex);
            }
        }

        public async Task DeleteNewsArticleAsync(string articleId)
        {
            try
            {
                var article = await this._dbContext.NewsArticles
                    .FirstOrDefaultAsync(a => a.ArticleId == articleId);

                if (article == null)
                {
                    throw new KeyNotFoundException($"Article with ID {articleId} not found.");
                }

                this._dbContext.NewsArticles.Remove(article);
                await this._dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while deleting news article.", ex);
            }
        }

        public async Task<NewsArticle> GetNewsArticleByIdAsync(string articleId)
        {
            try
            {
                var article = await this._dbContext.NewsArticles
                    .Include(a => a.RelatedStocks)
                    .FirstOrDefaultAsync(a => a.ArticleId == articleId);

                if (article == null)
                {
                    throw new KeyNotFoundException($"Article with ID {articleId} not found.");
                }

                return article;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving news article.", ex);
            }
        }

        public async Task<List<NewsArticle>> GetAllNewsArticlesAsync()
        {
            try
            {
                return await this._dbContext.NewsArticles
                    .Include(a => a.RelatedStocks)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving all news articles.", ex);
            }
        }

        public async Task<List<NewsArticle>> GetNewsArticlesByAuthorCNPAsync(string authorCNP)
        {
            try
            {
                return await this._dbContext.NewsArticles
                    .Where(a => a.AuthorCNP == authorCNP)
                    .Include(a => a.RelatedStocks)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving news articles by author CNP.", ex);
            }
        }

        public async Task<List<NewsArticle>> GetNewsArticlesByCategoryAsync(string category)
        {
            try
            {
                return await this._dbContext.NewsArticles
                    .Where(a => a.Category == category)
                    .Include(a => a.RelatedStocks)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving news articles by category.", ex);
            }
        }

        public async Task<List<NewsArticle>> GetNewsArticlesByStockAsync(string stockName)
        {
            try
            {
                return await this._dbContext.NewsArticles
                    .Where(a => a.RelatedStocks.Any(rs => rs.Name == stockName))
                    .Include(a => a.RelatedStocks)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving news articles by stock.", ex);
            }
        }

        public async Task MarkArticleAsReadAsync(string articleId)
        {
            try
            {
                var article = await this.GetNewsArticleByIdAsync(articleId);
                article.IsRead = true;
                await this.UpdateNewsArticleAsync(article);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while marking article as read.", ex);
            }
        }
    }
}