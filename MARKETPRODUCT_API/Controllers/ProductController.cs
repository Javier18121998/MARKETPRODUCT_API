using MARKETPRODUCT_API.Data.EFModels;
using MARKETPRODUCT_API.Data;
using Microsoft.AspNetCore.Mvc;
using MARKETPRODUCT_API.Messaging.MessageProducer;
using MARKETPRODUCT_API.Messaging.MessageModels;
using MARKETPRODUCT_API.Services.IServices;

namespace MARKETPRODUCT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            var products = _productService.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product = _productService.GetProduct(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<Product> CreateProduct([FromBody] Product newProduct)
        {
            if (newProduct == null)
            {
                return BadRequest();
            }

            var createdProduct = _productService.CreateProduct(newProduct);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            try
            {
                _productService.UpdateProduct(id, updatedProduct);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                _productService.DeleteProduct(id);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return NoContent();
        }
    }
}
