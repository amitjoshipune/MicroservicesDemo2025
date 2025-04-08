using OrderServices.Models;

namespace OrderServices.Contracts
{
    public interface IOrderService
    {
        Task<Product> GetProductDetails(int productId);
        Task<decimal> GetDiscount(int productId);

    }
}
