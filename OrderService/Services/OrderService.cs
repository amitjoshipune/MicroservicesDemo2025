using OrderServices.Contracts;
using OrderServices.Models;

namespace OrderServices.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Product> GetProductDetails(int productId)
        {
            //var response = await _httpClient.GetAsync($"https://productservice/api/products/{productId}");
            var response = await _httpClient.GetAsync($"https://localhost:7039/api/products/{productId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Product>();
        }

        public async Task<decimal> GetDiscount(int productId)
        {
            //var response = await _httpClient.GetAsync($"https://discountservice/api/discounts/{productId}");
            var response = await _httpClient.GetAsync($"https://localhost:7254/api/discounts/{productId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<decimal>();
        }
    }
}
