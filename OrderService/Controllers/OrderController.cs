using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderServices.Contracts;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("place-order/{productId}")]
    public async Task<IActionResult> PlaceOrder(int productId)
    {
        var product = await _orderService.GetProductDetails(productId);
        var discount = await _orderService.GetDiscount(productId);

        return Ok(new { Product = product, Discount = discount });
    }
}