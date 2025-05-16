namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Repositories;

    public class ZodiacService : IZodiacService
    {
        private readonly IUserRepository userRepository;
        private static readonly Random Random = new Random();

        public ZodiacService(IUserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        private static bool FlipCoin()
        {
            return Random.Next(2) == 0;
        }

        private static int ComputeJokeAsciiModulo10(string joke)
        {
            int jokeCharacterSum = 0;

            if (joke == null)
            {
                throw new ArgumentNullException(nameof(joke));
            }

            foreach (char character in joke)
            {
                jokeCharacterSum += character;
            }

            return jokeCharacterSum % 10;
        }

        public async Task CreditScoreModificationBasedOnJokeAndCoinFlipAsync()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage jokeApiResponse = await client.GetAsync("https://api.chucknorris.io/jokes/random");

            if (!jokeApiResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch joke from API.");
            }

            string jsonResponse = await jokeApiResponse.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(jsonResponse);
            string joke = doc.RootElement.GetProperty("value").GetString();

            int asciiJokeModulo10 = ComputeJokeAsciiModulo10(joke);
            List<User> users = await this.userRepository.GetAllAsync();
            bool flip = FlipCoin();

            foreach (User user in users)
            {
                if (flip)
                {
                    user.CreditScore += asciiJokeModulo10;
                }
                else
                {
                    user.CreditScore -= asciiJokeModulo10;
                }

                await this.userRepository.UpdateAsync(user.Id, user);
            }
        }

        private static int ComputeGravity()
        {
            return Random.Next(-10, 11);
        }

        public async void CreditScoreModificationBasedOnAttributeAndGravity()
        {
            List<User> userList = await this.userRepository.GetAllAsync();

            if (userList == null || userList.Count == 0)
            {
                throw new Exception("No users found.");
            }

            foreach (User user in userList)
            {
                int gravityResult = ComputeGravity();
                user.CreditScore += gravityResult;
                await this.userRepository.UpdateAsync(user.Id, user);
            }
        }
    }
}
