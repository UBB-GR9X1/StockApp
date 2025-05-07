using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Src.Model;

namespace StockApp.Services
{
    public class BillSplitReportApiService : IBillSplitReportApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7001/api/BillSplitReports";
        private readonly JsonSerializerOptions _jsonOptions;

        public BillSplitReportApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<BillSplitReport>> GetAllReportsAsync()
        {
            try
            {
                var reports = await _httpClient.GetFromJsonAsync<List<BillSplitReport>>(_baseUrl, _jsonOptions);
                return reports ?? new List<BillSplitReport>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while retrieving bill split reports from the API.", ex);
            }
        }

        public async Task<BillSplitReport> GetReportByIdAsync(int id)
        {
            try
            {
                var url = $"{_baseUrl}/{id}";
                var report = await _httpClient.GetFromJsonAsync<BillSplitReport>(url, _jsonOptions);
                
                if (report == null)
                {
                    throw new KeyNotFoundException($"Bill split report with ID '{id}' not found.");
                }
                
                return report;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error occurred while retrieving bill split report with ID '{id}' from the API.", ex);
            }
        }

        public async Task<BillSplitReport> CreateReportAsync(BillSplitReport report)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, report);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<BillSplitReport>(_jsonOptions) ?? 
                           throw new Exception("Failed to deserialize the created report.");
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create bill split report. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while creating bill split report via the API.", ex);
            }
        }

        public async Task<BillSplitReport> UpdateReportAsync(BillSplitReport report)
        {
            try
            {
                var url = $"{_baseUrl}/{report.Id}";
                var response = await _httpClient.PutAsJsonAsync(url, report);
                
                if (response.IsSuccessStatusCode)
                {
                    // For updates, the API returns 204 No Content, so we just return the object we sent
                    return report;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update bill split report. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while updating bill split report with ID '{report.Id}' via the API.", ex);
            }
        }

        public async Task<bool> DeleteReportAsync(int id)
        {
            try
            {
                var url = $"{_baseUrl}/{id}";
                var response = await _httpClient.DeleteAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete bill split report. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while deleting bill split report with ID '{id}' via the API.", ex);
            }
        }

        public async Task<int> GetCurrentBalanceAsync(string userCnp)
        {
            try
            {
                var url = $"{_baseUrl}/balance/{userCnp}";
                return await _httpClient.GetFromJsonAsync<int>(url, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while retrieving balance for user '{userCnp}' from the API.", ex);
            }
        }

        public async Task<int> GetCreditScoreAsync(string userCnp)
        {
            try
            {
                var url = $"{_baseUrl}/creditScore/{userCnp}";
                return await _httpClient.GetFromJsonAsync<int>(url, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while retrieving credit score for user '{userCnp}' from the API.", ex);
            }
        }

        public async Task UpdateCreditScoreAsync(string userCnp, int newCreditScore)
        {
            try
            {
                var url = $"{_baseUrl}/creditScore/{userCnp}";
                var response = await _httpClient.PutAsJsonAsync(url, newCreditScore);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to update credit score. Status code: {response.StatusCode}, Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while updating credit score for user '{userCnp}' via the API.", ex);
            }
        }
    }
} 