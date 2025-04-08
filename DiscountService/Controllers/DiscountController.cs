using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/discounts")]
public class DiscountController : ControllerBase
{
    private static readonly Dictionary<int, decimal> _discounts = new()
    {
        { 1, 10 },
        { 2, 5 }
    };

    [HttpGet("{productId}")]
    public IActionResult GetDiscount(int productId)
    {
        return _discounts.TryGetValue(productId, out var discount)
            ? Ok(discount)
            : NotFound();
    }
}