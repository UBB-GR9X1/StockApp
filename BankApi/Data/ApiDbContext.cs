namespace BankApi.Data
{
    using BankApi.Models;
    using BankApi.Models.Articles;
    using Microsoft.EntityFrameworkCore;

    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        public DbSet<BaseStock> BaseStocks { get; set; }

        public DbSet<NewsArticle> NewsArticles { get; set; }

        public DbSet<UserArticle> UserArticles { get; set; }

        public DbSet<NewsArticleStock> NewsArticleStocks { get; set; }

        private static void ConfigureBaseArticleEntity<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseArticle
        {
            modelBuilder.Entity<TEntity>()
                .HasKey(article => article.Id);

            modelBuilder.Entity<TEntity>()
                .Property(article => article.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TEntity>()
                .Property(article => article.Title)
                .IsRequired()
                .HasMaxLength(64);

            modelBuilder.Entity<TEntity>()
                .Property(article => article.Summary)
                .IsRequired()
                .HasMaxLength(64);

            modelBuilder.Entity<TEntity>()
                .Property(article => article.Content)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder.Entity<TEntity>()
                .Property(article => article.PublishedOn)
                .IsRequired()
                .HasColumnType("datetime2");
        }

        private static void ConfigureNewsArticleEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NewsArticle>().ToTable("NEWS_ARTICLE");

            ConfigureBaseArticleEntity<NewsArticle>(modelBuilder);

            modelBuilder.Entity<NewsArticle>()
                .Property(article => article.Source)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder.Entity<NewsArticle>()
                .Property(article => article.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            modelBuilder.Entity<NewsArticle>()
                .Property(article => article.IsWatchlistRelated)
                .IsRequired()
                .HasDefaultValue(false);

            modelBuilder.Entity<NewsArticle>()
                .Property(article => article.Category)
                .IsRequired(false)
                .HasMaxLength(256)
                .HasDefaultValue(null);
        }

        private static void ConfigureUserArticleEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserArticle>().ToTable("USER_ARTICLE");

            ConfigureBaseArticleEntity<UserArticle>(modelBuilder);

            modelBuilder.Entity<UserArticle>()
                .HasOne(article => article.Author)
                .WithMany(user => user.RelatedArticles)
                .HasForeignKey(article => article.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserArticle>()
                .Property(article => article.Topic)
                .IsRequired()
                .HasMaxLength(32);

            modelBuilder.Entity<UserArticle>()
                .Property(article => article.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(Status.Pending);
        }

        private static void ConfigureNewsArticleStockEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NewsArticleStock>().ToTable("NEWS_ARTICLE_STOCK");

            modelBuilder.Entity<NewsArticleStock>()
                .HasKey(ns => new { ns.ArticleId, ns.StockId }); 

            modelBuilder.Entity<NewsArticleStock>()
                .HasOne(ns => ns.Article)  
                .WithMany(a => a.RelatedStocks)
                .HasForeignKey(ns => ns.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NewsArticleStock>()
                .HasOne(ns => ns.Stock)  
                .WithMany(s => s.AssociatedNewsArticles)
                .HasForeignKey(ns => ns.StockId)
                .OnDelete(DeleteBehavior.Cascade); 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure BaseStock entity
            modelBuilder.Entity<BaseStock>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<BaseStock>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<BaseStock>()
                .Property(s => s.Name)
                .IsRequired();

            modelBuilder.Entity<BaseStock>()
                .Property(s => s.Symbol)
                .IsRequired();

            modelBuilder.Entity<BaseStock>()
                .Property(s => s.AuthorCNP)
                .IsRequired();

            // Configure the article entities.
            ConfigureNewsArticleEntity(modelBuilder);
            ConfigureUserArticleEntity(modelBuilder);

            ConfigureNewsArticleStockEntity(modelBuilder);
        }
    }
}
