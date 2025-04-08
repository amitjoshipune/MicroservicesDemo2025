using System.Threading.Tasks;

namespace TestRateLimitter
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            await TestRateLimit();


            Console.ReadLine();
        }

        public static async Task<Task<int>> TestRateLimit()
        {
            var client = new HttpClient();
            var url = "https://localhost:7289/api/orders/place-order/1";

            for (int i = 0; i < 2000; i++) // Sending 200 requests
            {
                await Task.Delay(50);
                var response = await client.GetAsync(url);
                Console.WriteLine($"Request {i + 1}: {response.StatusCode}");
                await Task.Delay(10);
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? 0;
                    Console.WriteLine($"Rate limit exceeded. Retry after {retryAfter} seconds.");
                    await Task.Delay(50);
                }
                else
                   if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var contentstring = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"CONTETS:{contentstring}");
                    } else 
                {
                    Console.WriteLine($"STATUS CODE={response.StatusCode}");
                }

            }

            return Task.FromResult(0);
        }
    }
}
