using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Models;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 1000 },
        new Product { Id = 2, Name = "Phone", Price = 500 }
    };

    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return product != null ? Ok(product) : NotFound();
    }
}
