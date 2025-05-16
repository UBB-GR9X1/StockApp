namespace StockApp.Repositories.Api
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Common.Models;

    public class HistoryProxyRepository(HttpClient httpClient) : IHistoryRepository
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public event EventHandler<string>? ErrorOccurred;

        public event EventHandler<bool>? LoadingStateChanged;

        public async Task<List<CreditScoreHistory>> GetAllHistoryAsync()
        {
            try
            {
                this.LoadingStateChanged?.Invoke(this, true);
                var response = await this._httpClient.GetFromJsonAsync<List<CreditScoreHistory>>($"api/history");
                return response ?? new List<CreditScoreHistory>();
            }
            catch (Exception ex)
            {
                this.ErrorOccurred?.Invoke(this, "Failed to retrieve all credit score history: " + ex.Message);
                throw;
            }
            finally
            {
                this.LoadingStateChanged?.Invoke(this, false);
            }
        }

        public async Task<CreditScoreHistory> GetHistoryByIdAsync(int id)
        {
            try
            {
                this.LoadingStateChanged?.Invoke(this, true);
                var response = await this._httpClient.GetFromJsonAsync<CreditScoreHistory>($"api/history/{id}");
                return response ?? throw new KeyNotFoundException($"History entry with ID {id} not found.");
            }
            catch (Exception ex)
            {
                this.ErrorOccurred?.Invoke(this, $"Failed to retrieve history entry with ID {id}: " + ex.Message);
                throw;
            }
            finally
            {
                this.LoadingStateChanged?.Invoke(this, false);
            }
        }

        public async Task AddHistoryAsync(CreditScoreHistory history)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }

            try
            {
                this.LoadingStateChanged?.Invoke(this, true);
                var response = await this._httpClient.PostAsJsonAsync($"api/history", history);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                this.ErrorOccurred?.Invoke(this, "Failed to add credit score history entry: " + ex.Message);
                throw;
            }
            finally
            {
                this.LoadingStateChanged?.Invoke(this, false);
            }
        }

        public async Task UpdateHistoryAsync(CreditScoreHistory history)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }

            try
            {
                this.LoadingStateChanged?.Invoke(this, true);
                var response = await this._httpClient.PutAsJsonAsync($"api/history/{history.Id}", history);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                this.ErrorOccurred?.Invoke(this, $"Failed to update history entry with ID {history.Id}: " + ex.Message);
                throw;
            }
            finally
            {
                this.LoadingStateChanged?.Invoke(this, false);
            }
        }

        public async Task DeleteHistoryAsync(int id)
        {
            try
            {
                this.LoadingStateChanged?.Invoke(this, true);
                var response = await this._httpClient.DeleteAsync($"api/history/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                this.ErrorOccurred?.Invoke(this, $"Failed to delete history entry with ID {id}: " + ex.Message);
                throw;
            }
            finally
            {
                this.LoadingStateChanged?.Invoke(this, false);
            }
        }

        public async Task<List<CreditScoreHistory>> GetHistoryForUserAsync(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
            {
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));
            }

            try
            {
                this.LoadingStateChanged?.Invoke(this, true);
                var response = await this._httpClient.GetFromJsonAsync<List<CreditScoreHistory>>($"api/history/user/{userCnp}");
                return response ?? new List<CreditScoreHistory>();
            }
            catch (Exception ex)
            {
                this.ErrorOccurred?.Invoke(this, "Failed to retrieve credit score history for user: " + ex.Message);
                throw;
            }
            finally
            {
                this.LoadingStateChanged?.Invoke(this, false);
            }
        }

        public async Task<List<CreditScoreHistory>> GetHistoryWeeklyAsync(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
            {
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));
            }

            try
            {
                this.LoadingStateChanged?.Invoke(this, true);
                var response = await this._httpClient.GetFromJsonAsync<List<CreditScoreHistory>>($"api/history/user/{userCnp}/weekly");
                return response ?? new List<CreditScoreHistory>();
            }
            catch (Exception ex)
            {
                this.ErrorOccurred?.Invoke(this, "Failed to retrieve weekly credit score history: " + ex.Message);
                throw;
            }
            finally
            {
                this.LoadingStateChanged?.Invoke(this, false);
            }
        }

        public async Task<List<CreditScoreHistory>> GetHistoryMonthlyAsync(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
            {
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));
            }

            try
            {
                this.LoadingStateChanged?.Invoke(this, true);
                var response = await this._httpClient.GetFromJsonAsync<List<CreditScoreHistory>>($"api/history/user/{userCnp}/monthly");
                return response ?? new List<CreditScoreHistory>();
            }
            catch (Exception ex)
            {
                this.ErrorOccurred?.Invoke(this, "Failed to retrieve monthly credit score history: " + ex.Message);
                throw;
            }
            finally
            {
                this.LoadingStateChanged?.Invoke(this, false);
            }
        }

        public async Task<List<CreditScoreHistory>> GetHistoryYearlyAsync(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
            {
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));
            }

            try
            {
                this.LoadingStateChanged?.Invoke(this, true);
                var response = await this._httpClient.GetFromJsonAsync<List<CreditScoreHistory>>($"api/history/user/{userCnp}/yearly");
                return response ?? new List<CreditScoreHistory>();
            }
            catch (Exception ex)
            {
                this.ErrorOccurred?.Invoke(this, "Failed to retrieve yearly credit score history: " + ex.Message);
                throw;
            }
            finally
            {
                this.LoadingStateChanged?.Invoke(this, false);
            }
        }
    }
}