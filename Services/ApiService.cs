namespace StockApp.Services
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides methods for interacting with an API using HTTP requests.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ApiService"/> class.
    /// </remarks>
    /// <param name="httpClient">The HTTP client used to send requests.</param>
    public class ApiService(HttpClient httpClient)
    {
        /// <summary>
        /// Occurs when an error happens during an API call.
        /// </summary>
        public event EventHandler<string>? OnError;

        /// <summary>
        /// Occurs when an API call starts or ends.
        /// </summary>
        public event EventHandler<bool>? OnLoad;

        /// <summary>
        /// Adds a handler for the <see cref="OnLoad"/> event.
        /// </summary>
        /// <param name="handler">The event handler to add.</param>
        public void AddLoadHandler(EventHandler<bool> handler) => this.OnLoad += handler;

        /// <summary>
        /// Adds a handler for the <see cref="OnError"/> event.
        /// </summary>
        /// <param name="handler">The event handler to add.</param>
        public void AddErrorHandler(EventHandler<string> handler) => this.OnError += handler;

        /// <summary>
        /// Sends a GET request to the specified endpoint and deserializes the response to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the response will be deserialized.</typeparam>
        /// <param name="endpoint">The API endpoint to send the GET request to.</param>
        /// <returns>The deserialized response, or the default value of <typeparamref name="T"/> if an error occurs.</returns>
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                this.OnLoad?.Invoke(this, true);
                var response = await httpClient.GetFromJsonAsync<T>(endpoint);
                return response ?? default;
            }
            catch (Exception ex)
            {
                this.OnError?.Invoke(this, $"Error calling endpoint \"{endpoint}\": {ex.Message}");
                return default;
            }
            finally
            {
                this.OnLoad?.Invoke(this, false);
            }
        }

        /// <summary>
        /// Sends a POST request to the specified endpoint with the provided body.
        /// </summary>
        /// <param name="endpoint">The API endpoint to send the POST request to.</param>
        /// <param name="body">The body of the POST request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task PostAsync(string endpoint, object body)
        {
            try
            {
                this.OnLoad?.Invoke(this, true);
                var response = await httpClient.PostAsJsonAsync<object>(endpoint, body);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                this.OnError?.Invoke(this, $"Error calling endpoint \"{endpoint}\": {ex.Message}");
            }
            finally
            {
                this.OnLoad?.Invoke(this, false);
            }
        }

        /// <summary>
        /// Sends a PUT request to the specified endpoint with the provided body.
        /// </summary>
        /// <param name="endpoint">The API endpoint to send the PUT request to.</param>
        /// <param name="body">The body of the PUT request. If null, an empty body will be sent.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task PutAsync(string endpoint, object? body = null)
        {
            try
            {
                this.OnLoad?.Invoke(this, true);
                var response = body == null
                    ? await httpClient.PutAsync(endpoint, new StringContent("{}"))
                    : await httpClient.PutAsJsonAsync<object>(endpoint, body);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                this.OnError?.Invoke(this, $"Error calling endpoint \"{endpoint}\": {ex.Message}");
            }
            finally
            {
                this.OnLoad?.Invoke(this, false);
            }
        }

        /// <summary>
        /// Sends a DELETE request to the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The API endpoint to send the DELETE request to.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(string endpoint)
        {
            try
            {
                this.OnLoad?.Invoke(this, true);
                var response = await httpClient.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                this.OnError?.Invoke(this, $"Error calling endpoint \"{endpoint}\": {ex.Message}");
            }
            finally
            {
                this.OnLoad?.Invoke(this, false);
            }
        }
    }
}
