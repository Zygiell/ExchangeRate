using System.Diagnostics;

namespace ExchangeRatePerformanceTests
{
    public class Tester
    {
        // ExchangeRateAPI has to be running. Provide valid _baseUrl and _apiKey, than turn on project
        private const string _baseUrl = "https://localhost:7287";

        private const string _apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjIwMiIsImV4cCI6MTY2NzU1MDQzOCwiaXNzIjoiaHR0cDovL2N1cnJlbmN5ZXhjaGFuZ2UuY29tIiwiYXVkIjoiaHR0cDovL2N1cnJlbmN5ZXhjaGFuZ2UuY29tIn0.a97tDfgCZ7invbhUhjn31yGIfrB0kizI65-rt7zvo7Q";

        public async Task Run()
        {
            const int MAX_ITERATIONS = 2;
            const int MAX_PARALLEL_REQUESTS = 50;
            const int DELAY = 100;
            const int RANDOM_URLS = 25;

            string[] urls = GenerateUrls(RANDOM_URLS);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Bearer", _apiKey);

                for (var step = 1; step <= MAX_ITERATIONS; step++)
                {
                    Console.WriteLine($"Started iteration: {step}");

                    var tasks = new List<Task>();
                    for (int i = 0; i < MAX_PARALLEL_REQUESTS; i++)
                    {
                        var url = urls[i % urls.Length];
                        int j = i;

                        tasks.Add(Task.Run(async () =>
                        {
                            var stopWatch = Stopwatch.StartNew();
                            await httpClient.GetAsync(url);
                            Console.WriteLine($"{url}\n" +
                                     $"Step - {step}, Request - {j + 1}, Time - {stopWatch.Elapsed}");
                            stopWatch.Stop();
                        }));
                    }

                    await Task.WhenAll(tasks);
                    Console.WriteLine($"Completed Iteration: {step}");

                    await Task.Delay(DELAY);
                }
            }
        }

        private static string[] GenerateUrls(uint count)
        {
            List<string> urls = new List<string>();

            Random gen = new Random();

            var sourceCurrencies = new string[] { "PLN", "GBP" };

            for (int i = 1; i <= count; i++)
            {
                var randomDates = GetRandomDates(gen);
                var startDate = randomDates.Key.ToString("yyyy-MM-dd");
                var endDate = randomDates.Value.ToString("yyyy-MM-dd");

                urls.Add($"{_baseUrl}/api/ExchangeRate?Key=[{sourceCurrencies[i % 2]}]=EUR&startDate={startDate}&endDate={endDate}");
            }

            return urls.ToArray();
        }

        private static KeyValuePair<DateTime, DateTime> GetRandomDates(Random gen)
        {
            DateTime start = new DateTime(2020, 5, 1);

            int startDateRange = (DateTime.Today - start).Days;
            var startDate = start.AddDays(gen.Next(startDateRange));

            int endDateRange = (DateTime.Today - startDate).Days;
            var endDate = startDate.AddDays(gen.Next(endDateRange));

            return new KeyValuePair<DateTime, DateTime>(startDate, endDate);
        }
    }
}