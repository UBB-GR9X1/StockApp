using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Src.Model;

namespace StockApp.Repositories
{
    /// <summary>
    /// Proxy repository that implements IBillSplitReportRepository to make calls to the BankAPI
    /// </summary>
    public class BillSplitReportProxyRepository : IBillSplitReportRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/BillSplitReport";
        private readonly JsonSerializerOptions _jsonOptions;

        public BillSplitReportProxyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <inheritdoc/>
        public async Task<List<BillSplitReport>> GetAllReportsAsync()
        {
            try
            {
                var reports = await _httpClient.GetFromJsonAsync<List<BillSplitReport>>(_baseUrl, _jsonOptions);
                return reports ?? new List<BillSplitReport>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting all reports: {ex.Message}");
                throw new Exception("Error occurred while retrieving bill split reports from the API.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<BillSplitReport> GetReportByIdAsync(int id)
        {
            try
            {
                var url = $"{_baseUrl}/{id}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<BillSplitReport>(_jsonOptions);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException($"Bill split report with ID {id} not found.");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve bill split report. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting report by id: {ex.Message}");
                throw new Exception($"Error occurred while retrieving the bill split report with ID {id} from the API.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<BillSplitReport> AddReportAsync(BillSplitReport report)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, report);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<BillSplitReport>(_jsonOptions);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to add bill split report. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating report: {ex.Message}");
                throw new Exception("Error occurred while adding the bill split report to the API.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<BillSplitReport> UpdateReportAsync(BillSplitReport report)
        {
            try
            {
                var url = $"{_baseUrl}/{report.Id}";
                var response = await _httpClient.PutAsJsonAsync(url, report);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<BillSplitReport>(_jsonOptions);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException($"Bill split report with ID {report.Id} not found.");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update bill split report. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating report: {ex.Message}");
                throw new Exception($"Error occurred while updating the bill split report with ID {report.Id} in the API.", ex);
            }
        }

        /// <inheritdoc/>
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
                System.Diagnostics.Debug.WriteLine($"Error deleting report: {ex.Message}");
                throw new Exception($"Error occurred while deleting the bill split report with ID {id} from the API.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> GetCurrentBalanceAsync(string userCnp)
        {
            try
            {
                var url = $"{_baseUrl}/balance/{userCnp}";
                return await _httpClient.GetFromJsonAsync<int>(url, _jsonOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting current balance: {ex.Message}");
                throw new Exception($"Error occurred while retrieving current balance for user {userCnp} from the API.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<float> SumTransactionsSinceReportAsync(string userCnp, DateTime sinceDate)
        {
            try
            {
                var url = $"{_baseUrl}/transactions/{userCnp}?since={sinceDate:yyyy-MM-dd}";
                return await _httpClient.GetFromJsonAsync<float>(url, _jsonOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error summing transactions: {ex.Message}");
                throw new Exception($"Error occurred while summing transactions for user {userCnp} since {sinceDate} from the API.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> GetCurrentCreditScoreAsync(string userCnp)
        {
            try
            {
                var url = $"{_baseUrl}/creditscore/{userCnp}";
                return await _httpClient.GetFromJsonAsync<int>(url, _jsonOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting credit score: {ex.Message}");
                throw new Exception($"Error occurred while retrieving credit score for user {userCnp} from the API.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task UpdateCreditScoreAsync(string userCnp, int newCreditScore)
        {
            try
            {
                var url = $"{_baseUrl}/creditscore/{userCnp}";
                var response = await _httpClient.PutAsJsonAsync(url, newCreditScore);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to update credit score. Status code: {response.StatusCode}, Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating credit score: {ex.Message}");
                throw new Exception($"Error occurred while updating credit score for user {userCnp} in the API.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task IncrementBillSharesPaidAsync(string userCnp)
        {
            try
            {
                var url = $"{_baseUrl}/incrementpaid/{userCnp}";
                var response = await _httpClient.PostAsync(url, null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to increment bill shares paid. Status code: {response.StatusCode}, Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error incrementing bill shares paid: {ex.Message}");
                throw new Exception($"Error occurred while incrementing bill shares paid for user {userCnp} in the API.", ex);
            }
        }
    }
}