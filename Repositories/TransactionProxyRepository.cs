namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using StockApp.Models;

    public class TransactionProxyRepository : ITransactionRepository
    {
        private readonly HttpClient _httpClient;

        public TransactionProxyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task AddTransaction(TransactionLogTransaction transaction)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Transaction", transaction);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to add transaction. Status code: {response.StatusCode}, Error: {errorContent}");

            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while adding the Transaction to the API");
            }
        }

        public async Task<List<TransactionLogTransaction>> getAllTransactions()
        {
            try
            {
                var transactions = await _httpClient.GetFromJsonAsync<List<TransactionLogTransaction>>("api/Transaction");
                return transactions ?? new List<TransactionLogTransaction>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while revrieving transactions from the API", ex);
            }
        }

        public async Task<List<TransactionLogTransaction>> GetByFilterCriteria(TransactionFilterCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria), "Filter criteria cannot be null.");
            }

            try
            {
                var url = $"api/Transaction/filter";
                var response = await _httpClient.PostAsJsonAsync(url, criteria);

                if (response.IsSuccessStatusCode)
                {
                    var transactions = await response.Content.ReadFromJsonAsync<List<TransactionLogTransaction>>();
                    return transactions ?? new List<TransactionLogTransaction>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve transactions. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while retrieving transactions by filter criteria from the API", ex);
            }
        }

    }
}
