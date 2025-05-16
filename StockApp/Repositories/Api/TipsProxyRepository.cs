namespace StockApp.Repositories.Api
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Common.Models;
    using StockApp.Repositories;

    /// <summary>
    /// Proxy repository for Tips that makes HTTP calls to the Tip API.
    /// </summary>
    public class TipsProxyRepository : ITipsRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/tip";

        public TipsProxyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Retrieves all tips for a given user.
        /// </summary>
        /// <param name="userCnp">The unique user CNP identifier.</param>
        /// <returns>A list of tips for the user.</returns>
        public async Task<List<Tip>> GetTipsForGivenUserAsync(string userCnp)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{userCnp}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Tip>>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to retrieve tips for user {userCnp}. Status code: {response.StatusCode}, Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while retrieving tips for user {userCnp}.", ex);
            }
        }

        /// <summary>
        /// Assigns a low-credit tip to the user.
        /// </summary>
        /// <param name="userCnp">The unique user CNP identifier.</param>
        /// <returns>The assigned tip.</returns>
        public async Task<GivenTip> GiveLowBracketTipAsync(string userCnp)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/low/{userCnp}", null);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<GivenTip>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to give low bracket tip to user {userCnp}. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while assigning low bracket tip to user {userCnp}.", ex);
            }
        }

        /// <summary>
        /// Assigns a medium-credit tip to the user.
        /// </summary>
        /// <param name="userCnp">The unique user CNP identifier.</param>
        /// <returns>The assigned tip.</returns>
        public async Task<GivenTip> GiveMediumBracketTipAsync(string userCnp)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/medium/{userCnp}", null);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<GivenTip>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to give medium bracket tip to user {userCnp}. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while assigning medium bracket tip to user {userCnp}.", ex);
            }
        }

        /// <summary>
        /// Assigns a high-credit tip to the user.
        /// </summary>
        /// <param name="userCnp">The unique user CNP identifier.</param>
        /// <returns>The assigned tip.</returns>
        public async Task<GivenTip> GiveHighBracketTipAsync(string userCnp)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/high/{userCnp}", null);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<GivenTip>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to give high bracket tip to user {userCnp}. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while assigning high bracket tip to user {userCnp}.", ex);
            }
        }
    }
}
