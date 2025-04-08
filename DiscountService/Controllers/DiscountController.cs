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
    public async Task<IActionResult> GetDiscountAsync(int productId)
    {
        await Task.Delay(50);

        // Simulate a transient failure
        var currentTime = DateTime.Now.Second;
        if (currentTime % 4 == 0) // Fail randomly 1/3 of the time
        {
            throw new HttpRequestException("Discount/GetDiscountAsync Simulated transient failure");
        }

        return _discounts.TryGetValue(productId, out var discount)
            ? Ok(discount)
            : NotFound();
    }
}