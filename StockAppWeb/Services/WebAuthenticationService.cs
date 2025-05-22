using Common.Models;
using Common.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace StockAppWeb.Services
{
    public class WebAuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserSession? _currentUserSession;

        public event EventHandler<UserLoggedInEventArgs>? UserLoggedIn;
        public event EventHandler<UserLoggedOutEventArgs>? UserLoggedOut;

        public WebAuthenticationService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("BankApi")
                ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            // Try to restore session from session storage
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var tokenJson = session.GetString("UserSession");
                if (!string.IsNullOrEmpty(tokenJson))
                {
                    _currentUserSession = JsonSerializer.Deserialize<UserSession>(tokenJson);
                }
            }
        }

        public UserSession? GetCurrentUserSession()
        {
            // If session is expired, clear it
            if (_currentUserSession != null && !_currentUserSession.IsLoggedIn)
            {
                _currentUserSession = null;
                ClearSessionStorage();
            }

            return _currentUserSession;
        }

        public string? GetToken()
        {
            return _currentUserSession?.IsLoggedIn == true ? _currentUserSession.Token : null;
        }

        public string GetUserCNP()
        {
            if (_currentUserSession == null || !_currentUserSession.IsLoggedIn)
            {
                throw new InvalidOperationException("User is not authenticated");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(_currentUserSession.Token);
            var cnpClaim = token.Claims.FirstOrDefault(c => c.Type == "cnp");

            return cnpClaim?.Value ?? throw new InvalidOperationException("CNP claim not found in token");
        }

        public bool IsUserAdmin()
        {
            return _currentUserSession?.IsAdmin ?? false;
        }

        public bool IsUserLoggedIn()
        {
            return _currentUserSession?.IsLoggedIn ?? false;
        }

        public async Task<UserSession> LoginAsync(string username, string password)
        {
            var loginRequest = new
            {
                Username = username,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Login failed: {response.ReasonPhrase}");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.Token))
            {
                throw new InvalidOperationException("Invalid token response");
            }

            // Parse the JWT to get claims
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenResponse.Token);

            // Extract user information from token claims
            var userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                ?? throw new InvalidOperationException("User ID not found in token claims");
            var userName = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                ?? throw new InvalidOperationException("User name not found in token claims");
            var roles = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            // Create and store the session
            _currentUserSession = new UserSession
            {
                UserId = userId,
                UserName = userName,
                Token = tokenResponse.Token,
                Roles = roles,
                ExpiryTimestamp = token.ValidTo
            };

            SaveToSessionStorage(_currentUserSession);
            OnUserLoggedIn(new UserLoggedInEventArgs(userId));

            return _currentUserSession;
        }

        public async Task LogoutAsync()
        {
            var userId = _currentUserSession?.UserId;

            // Clear session
            _currentUserSession = null;
            ClearSessionStorage();

            // Raise event if we had a user
            if (!string.IsNullOrEmpty(userId))
            {
                OnUserLoggedOut(new UserLoggedOutEventArgs(userId));
            }

            await Task.CompletedTask;
        }

        private void SaveToSessionStorage(UserSession session)
        {
            if (_httpContextAccessor.HttpContext?.Session != null)
            {
                var serializedSession = JsonSerializer.Serialize(session);
                _httpContextAccessor.HttpContext.Session.SetString("UserSession", serializedSession);
            }
        }

        private void ClearSessionStorage()
        {
            if (_httpContextAccessor.HttpContext?.Session != null)
            {
                _httpContextAccessor.HttpContext.Session.Remove("UserSession");
            }
        }

        protected virtual void OnUserLoggedIn(UserLoggedInEventArgs e)
        {
            UserLoggedIn?.Invoke(this, e);
        }

        protected virtual void OnUserLoggedOut(UserLoggedOutEventArgs e)
        {
            UserLoggedOut?.Invoke(this, e);
        }
    }

    // Helper class for deserializing token response
    internal class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}