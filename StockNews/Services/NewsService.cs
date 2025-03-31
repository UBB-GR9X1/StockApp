using StockNewsPage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockNewsPage.Services
{
    public class NewsService
    {
        private readonly AppState _appState;
        private readonly Dictionary<string, NewsArticle> _previewArticles = new();
        private readonly Dictionary<string, UserArticle> _previewUserArticles = new();
        private readonly List<NewsArticle> _cachedArticles = new();
        private static readonly List<UserArticle> _userArticles = new();
        private static bool _isInitialized = false;

        public NewsService()
        {
            _appState = AppState.Instance;

            if (!_isInitialized)
            {
                _userArticles.AddRange(GetMockUserArticles());
                _isInitialized = true;
            }
        }

        // Article Methods
        public async Task<List<NewsArticle>> GetNewsArticlesAsync()
        {
            await Task.Delay(300);

            var articles = GetMockArticles();
            // call the repository
            // return await _repository.GetNewsArticlesAsync(); or something like that

            // update cached articles
            UpdateCachedArticles(articles);
            // rn, return mock data
            return articles;
        }

        public async Task<NewsArticle> GetNewsArticleByIdAsync(string articleId)
        {
            // check if this is a preview article
            if (articleId.StartsWith("preview:"))
            {
                var previewId = articleId.Substring(8); // Remove "preview:"
                if (_previewArticles.TryGetValue(previewId, out var previewArticle))
                {
                    return previewArticle;
                }
            }

            await Task.Delay(200);

            // call the repository
            // return await _repository.GetNewsArticleByIdAsync(articleId); or something like that

            // check in cached articles first
            var cachedArticle = _cachedArticles.FirstOrDefault(a => a.ArticleId == articleId);
            if (cachedArticle != null)
            {
                return cachedArticle;
            }

            // if not in cache, get from mock data
            var mockArticles = GetMockArticles();
            return mockArticles.FirstOrDefault(a => a.ArticleId == articleId);
        }


        public async Task<bool> MarkArticleAsReadAsync(string articleId)
        {
            await Task.Delay(100);

            // call the repository
            // return await _repository.MarkArticleAsReadAsync(articleId); or something like that

            
            // mark as read in cached articles
            var article = _cachedArticles.FirstOrDefault(a => a.ArticleId == articleId);
            if (article != null)
            {
                article.IsRead = true;
            }
            // rn, return success
            return true;
        }

        public async Task<bool> CreateArticleAsync(NewsArticle article)
        {
            // ensure user is logged in
            if (_appState.CurrentUser == null)
            {
                throw new UnauthorizedAccessException("User must be logged in to create an article");
            }

            await Task.Delay(300);

            // call the repository
            // return await _repository.CreateArticleAsync(article); or something like that

            _cachedArticles.Add(article);
            // rn, return success
            return true;
        }

        // User Article Methods
        public async Task<List<UserArticle>> GetUserArticlesAsync(string status = null, string topic = null)
        {
            // ensure the user is admin
            if (_appState.CurrentUser == null || !_appState.CurrentUser.IsAdmin)
            {
                throw new UnauthorizedAccessException("User must be an admin to access user articles");
            }

            await Task.Delay(300);

            // call the repository
            // return await _repository.GetUserArticlesAsync(status, topic); or something like that

            // rn, return mock data
            var userArticles = new List<UserArticle>(_userArticles);

            // filters
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                userArticles = userArticles.Where(a => a.Status == status).ToList();
            }

            if (!string.IsNullOrEmpty(topic) && topic != "All")
            {
                userArticles = userArticles.Where(a => a.Topic == topic).ToList();
            }

            return userArticles;
        }

        public async Task<bool> ApproveUserArticleAsync(string articleId)
        {
            // ensure the user is admin
            if (_appState.CurrentUser == null || !_appState.CurrentUser.IsAdmin)
            {
                throw new UnauthorizedAccessException("User must be an admin to approve articles");
            }

            await Task.Delay(300);

            // call the repository
            // return await _repository.ApproveUserArticleAsync(articleId); or something like that

            // rn, return true/false based on local data
            var article = _userArticles.FirstOrDefault(a => a.ArticleId == articleId);
            if (article != null)
            {
                article.Status = "Approved";
                _cachedArticles.Clear();
                return true;
            }

            return false;
        }

        public async Task<bool> RejectUserArticleAsync(string articleId)
        {
            // ensure the user is admin
            if (_appState.CurrentUser == null || !_appState.CurrentUser.IsAdmin)
            {
                throw new UnauthorizedAccessException("User must be an admin to reject articles");
            }

            await Task.Delay(300);

            // call the repository
            // return await _repository.RejectUserArticleAsync(articleId); or something like that

            // rn, return true/false based on local data
            var article = _userArticles.FirstOrDefault(a => a.ArticleId == articleId);
            if (article != null)
            {
                article.Status = "Rejected";
                _cachedArticles.Clear();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteUserArticleAsync(string articleId)
        {
            // ensure the user is admin
            if (_appState.CurrentUser == null || !_appState.CurrentUser.IsAdmin)
            {
                throw new UnauthorizedAccessException("User must be an admin to delete articles");
            }

            await Task.Delay(300);

            // call the repository
            // return await _repository.DeleteUserArticleAsync(articleId); or something like that

            // rn, return true/false based on local data
            var article = _userArticles.FirstOrDefault(a => a.ArticleId == articleId);
            if (article != null)
            {
                _userArticles.Remove(article);
                _cachedArticles.Clear();
                return true;
            }

            return false;
        }

        public async Task<bool> SubmitUserArticleAsync(UserArticle article)
        {
            // ensure user is logged in
            if (_appState.CurrentUser == null)
            {
                throw new UnauthorizedAccessException("User must be logged in to submit an article");
            }

            // set author and submission date
            article.Author = _appState.CurrentUser.Username;
            article.SubmissionDate = DateTime.Now;
            article.Status = "Pending";

            await Task.Delay(300);

            // call repository
            // return await _repository.SubmitUserArticleAsync(article); or something like that

            // rn, return success and add to local data
            _userArticles.Add(article);
            _cachedArticles.Clear();
            return true;
        }

        // User Authentication Methods
        public async Task<User> GetCurrentUserAsync()
        {
            // checks if user is already in app state
            if (_appState.CurrentUser != null)
            {
                return _appState.CurrentUser;
            }

            await Task.Delay(200);

            // call the repository
            // return await _repository.GetCurrentUserAsync(); or something like that

            // rn, return null (no user logged in)
            return null;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            await Task.Delay(300);

            // call the repository
            // return await _repository.LoginAsync(username, password); or something like that

            // rn, return mock user if credentials match, this needs some work since it's not async and i don't have access to the repository
            if (username == "admin" && password == "admin")
            {
                return new User
                {
                    UserId = "1",
                    Username = "admin",
                    Email = "admin@example.com",
                    IsAdmin = true,
                    Roles = new[] { "Admin", "User" }
                };
            }
            else if (username == "user" && password == "user")
            {
                return new User
                {
                    UserId = "2",
                    Username = "user",
                    Email = "user@example.com",
                    IsAdmin = false,
                    Roles = new[] { "User" }
                };
            }

            return null;
        }

        public void Logout()
        {
            // optional: call the repository to log out, if needed, not necessary, and i don't have access to the repository, so idk what should be done here
            // _repository.Logout(); or something like that

            // clear the current user from app state
            _appState.CurrentUser = null;
            // clear preview articles
            _previewArticles.Clear();
            _previewUserArticles.Clear();
        }

        // Preview Methods
        public void StorePreviewArticle(NewsArticle article, UserArticle userArticle)
        {
            _previewArticles[userArticle.ArticleId] = article;
            _previewUserArticles[userArticle.ArticleId] = userArticle;
        }

        public UserArticle GetUserArticleForPreview(string articleId)
        {
            if (_previewUserArticles.TryGetValue(articleId, out var previewArticle))
            {
                return previewArticle;
            }

            // if not in preview cache, check the regular user articles
            return _userArticles.FirstOrDefault(a => a.ArticleId == articleId);
        }

        public void UpdateCachedArticles(List<NewsArticle> articles)
        {
            _cachedArticles.Clear();
            if (articles != null)
            {
                _cachedArticles.AddRange(articles);
            }
        }

        public List<NewsArticle> GetCachedArticles()
        {
            return _cachedArticles.Count > 0 ? _cachedArticles : GetMockArticles();
        }

        // Mock Data Methods
        private List<NewsArticle> GetMockArticles()
        {
            // approved user articles to news articles
            var approvedUserArticles = _userArticles
                .Where(ua => ua.Status == "Approved")
                .Select(ua => new NewsArticle
                {
                    ArticleId = ua.ArticleId,
                    Title = ua.Title,
                    Summary = ua.Summary,
                    Content = ua.Content,
                    Source = $"User: {ua.Author}",
                    PublishedDate = ua.SubmissionDate.ToString("MMMM dd, yyyy"),
                    IsRead = false,
                    IsWatchlistRelated = false,
                    Category = ua.Topic,
                    RelatedStocks = ua.RelatedStocks
                })
                .ToList();

            // with built-in mock articles
            var mockArticles = new List<NewsArticle>
            {
                new NewsArticle
                {
                    ArticleId = "1",
                    Title = "Market Reaches All-Time High",
                    Summary = "The stock market reached an all-time high today as tech stocks surged.",
                    Content = "The stock market reached an all-time high today as tech stocks surged. Investors are optimistic about the future of technology companies as they continue to innovate and grow. The S&P 500 index rose 1.2% to close at a record high, while the Nasdaq Composite index gained 1.5%. Leading the gains were shares of major tech companies such as Apple, Microsoft, and Amazon, which all rose more than 2%. Analysts attribute the rally to strong earnings reports and positive economic data. \"The market is showing resilience despite concerns about inflation and interest rates,\" said John Smith, chief market strategist at XYZ Investment Firm. \"Tech companies are demonstrating their ability to adapt and thrive in the current economic environment.\" However, some experts caution that the market may be overvalued and due for a correction. \"We're seeing signs of froth in certain sectors,\" warned Jane Doe, portfolio manager at ABC Asset Management. \"Investors should be selective and focus on companies with strong fundamentals.\" The rally comes amid a backdrop of improving economic conditions, with unemployment falling and consumer spending rising. The Federal Reserve has signaled that it will maintain its accommodative monetary policy for the foreseeable future, providing further support for the market. Looking ahead, investors will be closely watching upcoming earnings reports and economic data for signs of continued growth or potential headwinds.",
                    Source = "Financial Times",
                    PublishedDate = "April 15, 2023",
                    IsRead = false,
                    IsWatchlistRelated = true,
                    Category = "Market Analysis",
                    RelatedStocks = new List<string> { "AAPL", "MSFT", "AMZN" }
                },
                new NewsArticle
                {
                    ArticleId = "2",
                    Title = "Tech Company Announces New Product Line",
                    Summary = "A major tech company has announced a new product line that is expected to revolutionize the industry.",
                    Content = "A major tech company has announced a new product line that is expected to revolutionize the industry. The company unveiled its latest innovations at a highly anticipated event yesterday, showcasing cutting-edge technology that promises to transform how consumers interact with their devices. The new products include a range of smart home devices, wearable technology, and advanced computing solutions. Industry analysts are bullish on the company's prospects following the announcement. \"This represents a significant leap forward in terms of both hardware and software integration,\" said tech analyst Sarah Johnson. \"The company has once again demonstrated its ability to innovate and stay ahead of the competition.\" The stock price of the company surged following the announcement, rising 5% in after-hours trading. Investors are particularly excited about the potential for new revenue streams from the expanded product ecosystem. Pre-orders for the new devices will begin next week, with shipping expected to start in early June. The company has also announced partnerships with several major retailers to ensure wide availability of the products at launch. Competitors are expected to respond with their own product announcements in the coming months, potentially setting up a fierce battle for market share in the second half of the year. \"This is just the beginning of what promises to be an exciting period of innovation in the tech sector,\" said industry consultant Michael Brown. \"Consumers will ultimately benefit from the increased competition and rapid pace of technological advancement.\"",
                    Source = "Tech Insider",
                    PublishedDate = "April 14, 2023",
                    IsRead = true,
                    IsWatchlistRelated = false,
                    Category = "Company News",
                    RelatedStocks = new List<string> { "AAPL", "GOOG" }
                },
                new NewsArticle
                {
                    ArticleId = "3",
                    Title = "Economic Growth Exceeds Expectations",
                    Summary = "The economy grew faster than expected in the first quarter, according to new data.",
                    Content = "The economy grew faster than expected in the first quarter, according to new data released by the Commerce Department today. Gross domestic product (GDP) increased at an annual rate of 3.2%, surpassing economists' forecasts of 2.5% growth. The stronger-than-expected growth was driven by robust consumer spending, business investment, and exports. Consumer spending, which accounts for more than two-thirds of economic activity, rose 3.5% as households increased purchases of durable goods and services. Business investment grew 5.2%, reflecting increased spending on equipment and intellectual property products. Exports surged 7.8%, outpacing imports which grew at a more modest 4.5%. The positive economic data has implications for monetary policy, as the Federal Reserve may need to reassess its interest rate projections. \"The strong GDP report suggests that the economy is on solid footing, which could lead the Fed to maintain higher interest rates for longer than previously anticipated,\" said economist Robert Johnson. Inflation, as measured by the personal consumption expenditures (PCE) price index, increased at a 2.7% rate in the first quarter, slightly above the Fed's 2% target. Core PCE, which excludes food and energy prices, rose 2.5%. The labor market remains tight, with the unemployment rate holding steady at 3.6%. Wage growth has moderated but remains above pre-pandemic levels. Looking ahead, economists expect growth to slow in the second quarter as the effects of higher interest rates continue to work their way through the economy. However, the risk of a recession in the near term has diminished given the strength of recent economic data.",
                    Source = "Economic Journal",
                    PublishedDate = "April 13, 2023",
                    IsRead = false,
                    IsWatchlistRelated = false,
                    Category = "Economic News",
                    RelatedStocks = new List<string> { "SPY", "DIA" }
                },
                new NewsArticle
                {
                    ArticleId = "4",
                    Title = "New Trading Feature Added to Platform",
                    Summary = "Our platform has added a new trading feature that allows for more efficient order execution.",
                    Content = "Our platform has added a new trading feature that allows for more efficient order execution. The new feature, called Smart Order Routing (SOR), automatically directs orders to the exchange or market center offering the best price at the time of the order. This ensures that traders get the best possible execution for their trades. SOR works by scanning multiple exchanges and market centers in real-time to find the best available price for a given security. It then routes the order to that venue for execution. If the order cannot be fully executed at a single venue, SOR will split the order and route it to multiple venues to achieve the best overall execution. \"This new feature represents a significant enhancement to our trading platform,\" said the company's Chief Technology Officer. \"It demonstrates our commitment to providing our users with the most advanced trading tools available.\" In addition to improving execution quality, SOR can also help reduce trading costs by minimizing market impact and slippage. It is particularly beneficial for large orders that might otherwise move the market if executed at a single venue. The feature is now available to all users of the platform at no additional cost. To access SOR, users simply need to select the \"Smart Routing\" option when placing an order. The company plans to release additional trading enhancements in the coming months, including advanced order types and improved risk management tools. These updates are part of a broader initiative to modernize the platform and provide users with a more comprehensive trading experience. \"We're constantly looking for ways to improve our platform and give our users an edge in the market,\" said the company's CEO. \"Smart Order Routing is just the beginning of what we have planned for this year.\"",
                    Source = "Platform News",
                    PublishedDate = "April 12, 2023",
                    IsRead = false,
                    IsWatchlistRelated = false,
                    Category = "Functionality News",
                    RelatedStocks = new List<string>()
                },
                new NewsArticle
                {
                    ArticleId = "5",
                    Title = "Pharmaceutical Company Receives FDA Approval",
                    Summary = "A major pharmaceutical company has received FDA approval for its new drug.",
                    Content = "A major pharmaceutical company has received FDA approval for its new drug targeting a rare genetic disorder. The approval comes after extensive clinical trials demonstrated the drug's safety and efficacy in treating the condition, which affects approximately 1 in 50,000 people worldwide. The drug, which will be marketed under the name Genetix, works by targeting a specific protein involved in the disease pathway. In clinical trials, patients who received the drug showed significant improvement in symptoms compared to those who received a placebo. \"This approval represents a major milestone for patients suffering from this debilitating disorder,\" said the company's Chief Medical Officer. \"Until now, treatment options have been limited to managing symptoms rather than addressing the underlying cause of the disease.\" The company expects to launch the drug in the U.S. market within the next three months, with international launches to follow pending regulatory approvals in other countries. Analysts estimate that the drug could generate annual sales of $1-2 billion at peak. The stock price of the pharmaceutical company rose 8% following the announcement, reflecting investor optimism about the drug's commercial potential. Industry experts note that the approval also validates the company's research and development strategy, which has focused on rare diseases with high unmet medical needs. \"This approval strengthens the company's position in the rare disease space and demonstrates its ability to successfully navigate the regulatory approval process,\" said healthcare analyst David Wilson. The company has already begun work on expanding the drug's indications to related disorders and is conducting early-stage research on next-generation treatments based on similar mechanisms of action.",
                    Source = "Health News",
                    PublishedDate = "April 11, 2023",
                    IsRead = true,
                    IsWatchlistRelated = true,
                    Category = "Company News",
                    RelatedStocks = new List<string> { "PFE", "JNJ", "MRK" }
                }
            };

            // add approved user articles to the list
            mockArticles.AddRange(approvedUserArticles);

            return mockArticles;
        }

        private List<UserArticle> GetMockUserArticles()
        {
            return new List<UserArticle>
            {
                new UserArticle
                {
                    ArticleId = "ua1",
                    Title = "Analysis of Recent Market Trends",
                    Summary = "A detailed analysis of recent market trends and their implications for investors.",
                    Content = "The market has shown significant volatility in recent weeks, with major indices experiencing both sharp declines and rapid recoveries. This pattern can be attributed to several factors, including uncertainty surrounding monetary policy, geopolitical tensions, and mixed economic data. Investors should consider diversifying their portfolios and focusing on sectors with strong fundamentals and growth potential. Value stocks may outperform growth stocks in the current environment, particularly if inflation remains elevated and interest rates continue to rise. However, selective opportunities in the technology sector still exist, especially among companies with robust cash flows and reasonable valuations. Small-cap stocks could present attractive opportunities for long-term investors willing to tolerate higher volatility.In conclusion, market participants should remain vigilant and adaptable in the current environment. By carefully analyzing economic indicators, company fundamentals, and market sentiment, investors can position themselves to navigate the challenges and capitalize on opportunities that arise in these dynamic market conditions.",
                    Author = "John Smith",
                    SubmissionDate = DateTime.Now.AddDays(-5),
                    Status = "Pending",
                    Topic = "Market Analysis",
                    RelatedStocks = new List<string> { "SPY", "QQQ", "IWM" }
                },
                new UserArticle
                {
                    ArticleId = "ua2",
                    Title = "The Future of Electric Vehicles",
                    Summary = "An exploration of the electric vehicle industry and its growth prospects.",
                    Content = "Electric vehicles (EVs) are rapidly transforming the automotive industry, with global sales growing at an unprecedented rate. Major automakers have committed billions of dollars to electrify their fleets, while new entrants are challenging established players with innovative designs and technologies. The shift to EVs is being driven by a combination of factors, including government regulations aimed at reducing carbon emissions, declining battery costs, and increasing consumer acceptance. As battery technology continues to improve and charging infrastructure expands, the barriers to EV adoption are steadily diminishing. Investors looking to capitalize on this trend should consider not only the vehicle manufacturers themselves but also companies involved in the EV supply chain, such as battery producers, semiconductor manufacturers, and charging network operators. The transition to electric mobility represents one of the most significant investment opportunities of the coming decade, with implications that extend far beyond the automotive sector to include utilities, energy storage, and smart grid technologies.",
                    Author = "Jane Doe",
                    SubmissionDate = DateTime.Now.AddDays(-3),
                    Status = "Approved",
                    Topic = "Company News",
                    RelatedStocks = new List<string> { "TSLA", "F", "GM" }
                },
                new UserArticle
                {
                    ArticleId = "ua3",
                    Title = "Cryptocurrency Market Update",
                    Summary = "A review of recent developments in the cryptocurrency market.",
                    Content = "The cryptocurrency market continues to evolve rapidly, with institutional adoption increasing despite ongoing regulatory scrutiny. Major financial institutions that once dismissed cryptocurrencies are now offering crypto-related services to their clients, reflecting the growing mainstream acceptance of digital assets. Bitcoin remains the dominant cryptocurrency by market capitalization, but Ethereum and other alternative coins have gained significant traction due to their smart contract capabilities and applications in decentralized finance (DeFi). Regulatory developments remain a key factor influencing the market, with authorities around the world working to establish frameworks that balance innovation with investor protection and financial stability. The recent approval of cryptocurrency ETFs in some jurisdictions has provided more accessible investment vehicles for traditional investors. However, the market still faces challenges, including concerns about energy consumption, security vulnerabilities, and price volatility. Investors should approach this asset class with caution and consider it as part of a diversified portfolio rather than a standalone investment.",
                    Author = "Michael Johnson",
                    SubmissionDate = DateTime.Now.AddDays(-2),
                    Status = "Rejected",
                    Topic = "Market Analysis",
                    RelatedStocks = new List<string> { "COIN", "MSTR", "SQ" }
                },
                new UserArticle
                {
                    ArticleId = "ua4",
                    Title = "The Impact of Artificial Intelligence on Financial Services",
                    Summary = "An analysis of how AI is transforming the financial services industry.",
                    Content = "Artificial intelligence (AI) is revolutionizing the financial services industry, enabling institutions to enhance customer experiences, improve risk management, and increase operational efficiency. From chatbots and robo-advisors to sophisticated fraud detection systems and algorithmic trading, AI applications are becoming increasingly prevalent across all segments of the industry. Financial institutions are leveraging machine learning algorithms to analyze vast amounts of data and derive actionable insights that inform business decisions. In wealth management, AI-powered platforms are democratizing access to financial advice by providing personalized recommendations at a fraction of the cost of traditional advisory services. Banks are using AI to streamline loan approval processes, assess creditworthiness more accurately, and detect potential fraud in real-time. Insurance companies are employing AI to improve underwriting, expedite claims processing, and develop more personalized products. As AI technology continues to advance, its impact on financial services will only grow, creating both opportunities and challenges for industry participants and investors alike.",
                    Author = "Sarah Williams",
                    SubmissionDate = DateTime.Now.AddDays(-1),
                    Status = "Pending",
                    Topic = "Functionality News",
                    RelatedStocks = new List<string> { "JPM", "GS", "MS" }
                }
            };
        }
    }
}

