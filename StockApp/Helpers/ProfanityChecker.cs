namespace Src.Helpers
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    static public class ProfanityChecker
    {
        private static readonly HttpClient Client = new HttpClient();

        static public async Task<bool> IsMessageOffensive(string messageToBeChecked)
        {
            try
            {
                string apiUrl = $"https://www.purgomalum.com/homepageService/containsprofanity?text={Uri.EscapeDataString(messageToBeChecked)}";
                HttpResponseMessage response = await Client.GetAsync(apiUrl);
                string result = await response.Content.ReadAsStringAsync();
                return result.Trim().ToLower() == "true";
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

