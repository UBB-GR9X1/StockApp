using Common.Models;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace StockApp.Services
{
    public class BillSplitReportProxyService(HttpClient httpClient) : IProxyService, IBillSplitReportService
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public async Task<List<BillSplitReport>> GetBillSplitReportsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<BillSplitReport>>("api/BillSplitReport") ??
                throw new InvalidOperationException("Failed to deserialize bill split reports response.");
        }

        public async Task<BillSplitReport?> GetBillSplitReportByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<BillSplitReport>($"api/BillSplitReport/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<BillSplitReport> CreateBillSplitReportAsync(BillSplitReport billSplitReport)
        {
            var response = await _httpClient.PostAsJsonAsync("api/BillSplitReport", billSplitReport);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BillSplitReport>() ??
                throw new InvalidOperationException("Failed to deserialize created bill split report response.");
        }

        public async Task<int> GetDaysOverdueAsync(BillSplitReport billSplitReport)
        {
            var response = await _httpClient.GetAsync($"api/BillSplitReport/{billSplitReport.Id}/daysOverdue");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task SolveBillSplitReportAsync(BillSplitReport billSplitReportToBeSolved)
        {
            var response = await _httpClient.PostAsync($"api/BillSplitReport/{billSplitReportToBeSolved.Id}/solve", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBillSplitReportAsync(BillSplitReport billSplitReportToBeSolved)
        {
            var response = await _httpClient.DeleteAsync($"api/BillSplitReport/{billSplitReportToBeSolved.Id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<BillSplitReport> UpdateBillSplitReportAsync(BillSplitReport billSplitReport)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/BillSplitReport/{billSplitReport.Id}", billSplitReport);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BillSplitReport>() ??
                throw new InvalidOperationException("Failed to deserialize updated bill split report response.");
        }

        // Additional helper methods that align with the controller's additional endpoints
        public async Task<List<BillSplitReport>> GetMyReportsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<BillSplitReport>>("api/BillSplitReport/my-reports") ??
                throw new InvalidOperationException("Failed to deserialize my bill split reports response.");
        }

        public async Task<List<BillSplitReport>> GetReportsByUserAsync(string userCnp)
        {
            return await _httpClient.GetFromJsonAsync<List<BillSplitReport>>($"api/BillSplitReport/user/{userCnp}") ??
                throw new InvalidOperationException("Failed to deserialize user's bill split reports response.");
        }
    }
}