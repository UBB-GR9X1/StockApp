using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using StockApp.Models;
using StockApp.Exceptions;

namespace StockApp.Repositories
{
    public class HistoryProxyRepo : IHistoryRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/history";

        public HistoryProxyRepo(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public List<CreditScoreHistory> GetAllHistory()
        {
            try
            {
                var response = _httpClient.GetAsync(BaseUrl).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadFromJsonAsync<List<CreditScoreHistory>>().Result ?? new List<CreditScoreHistory>();
            }
            catch (Exception ex)
            {
                throw new HistoryServiceException("Error retrieving all credit score history", ex);
            }
        }

        public CreditScoreHistory GetHistoryById(int id)
        {
            try
            {
                var response = _httpClient.GetAsync($"{BaseUrl}/{id}").Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadFromJsonAsync<CreditScoreHistory>().Result 
                    ?? throw new KeyNotFoundException($"History entry with ID {id} not found.");
            }
            catch (Exception ex)
            {
                throw new HistoryServiceException($"Error retrieving history entry with ID {id}", ex);
            }
        }

        public void AddHistory(CreditScoreHistory history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            try
            {
                var response = _httpClient.PostAsJsonAsync(BaseUrl, history).Result;
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new HistoryServiceException("Error adding credit score history entry", ex);
            }
        }

        public void UpdateHistory(CreditScoreHistory history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            try
            {
                var response = _httpClient.PutAsJsonAsync($"{BaseUrl}/{history.Id}", history).Result;
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new HistoryServiceException($"Error updating history entry with ID {history.Id}", ex);
            }
        }

        public void DeleteHistory(int id)
        {
            try
            {
                var response = _httpClient.DeleteAsync($"{BaseUrl}/{id}").Result;
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new HistoryServiceException($"Error deleting history entry with ID {id}", ex);
            }
        }

        public List<CreditScoreHistory> GetHistoryForUser(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));

            try
            {
                var response = _httpClient.GetAsync($"{BaseUrl}/user/{userCnp}").Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadFromJsonAsync<List<CreditScoreHistory>>().Result ?? new List<CreditScoreHistory>();
            }
            catch (Exception ex)
            {
                throw new HistoryServiceException("Error retrieving credit score history for user", ex);
            }
        }

        public List<CreditScoreHistory> GetHistoryWeekly(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));

            try
            {
                var response = _httpClient.GetAsync($"{BaseUrl}/user/{userCnp}/weekly").Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadFromJsonAsync<List<CreditScoreHistory>>().Result ?? new List<CreditScoreHistory>();
            }
            catch (Exception ex)
            {
                throw new HistoryServiceException("Error retrieving weekly credit score history", ex);
            }
        }

        public List<CreditScoreHistory> GetHistoryMonthly(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));

            try
            {
                var response = _httpClient.GetAsync($"{BaseUrl}/user/{userCnp}/monthly").Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadFromJsonAsync<List<CreditScoreHistory>>().Result ?? new List<CreditScoreHistory>();
            }
            catch (Exception ex)
            {
                throw new HistoryServiceException("Error retrieving monthly credit score history", ex);
            }
        }

        public List<CreditScoreHistory> GetHistoryYearly(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));

            try
            {
                var response = _httpClient.GetAsync($"{BaseUrl}/user/{userCnp}/yearly").Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadFromJsonAsync<List<CreditScoreHistory>>().Result ?? new List<CreditScoreHistory>();
            }
            catch (Exception ex)
            {
                throw new HistoryServiceException("Error retrieving yearly credit score history", ex);
            }
        }
    }
} 