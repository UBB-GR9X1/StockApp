using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Src.Model;

namespace StockApp.Services
{
    public class BillSplitReportApiService : IBillSplitReportApiService
    {
        private readonly HttpClient _httpClient;

        public BillSplitReportApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<BillSplitReport>> GetAllReportsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<BillSplitReport>>("api/BillSplitReport") 
                    ?? new List<BillSplitReport>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting all reports: {ex.Message}");
                return new List<BillSplitReport>();
            }
        }

        public async Task<BillSplitReport> GetReportByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<BillSplitReport>($"api/BillSplitReport/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting report by id: {ex.Message}");
                throw;
            }
        }

        public async Task<BillSplitReport> CreateReportAsync(BillSplitReport report)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/BillSplitReport", report);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<BillSplitReport>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating report: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateReportAsync(BillSplitReport report)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/BillSplitReport/{report.Id}", report);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating report: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteReportAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/BillSplitReport/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting report: {ex.Message}");
                return false;
            }
        }

        public async Task<int> GetCurrentBalanceAsync(string userCnp)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<int>($"api/BillSplitReport/balance/{userCnp}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting current balance: {ex.Message}");
                return 0; // Default to 0 on error
            }
        }

        public async Task<int> GetCreditScoreAsync(string userCnp)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<int>($"api/BillSplitReport/creditscore/{userCnp}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting credit score: {ex.Message}");
                return 0; // Default to 0 on error
            }
        }

        public async Task UpdateCreditScoreAsync(string userCnp, int newCreditScore)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/BillSplitReport/creditscore/{userCnp}", newCreditScore);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating credit score: {ex.Message}");
                throw;
            }
        }
    }
} 