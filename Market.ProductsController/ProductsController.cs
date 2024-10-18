using Market.BL.IBL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Market.Utilities.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Market.ProductsController
{
    /// <summary>
    /// Controller for handling product-related operations.
    /// </summary>
    public class ProductsController : MarketProductControllerBase<ProductsController>
    {
        private readonly IProductServiceBL _productServiceBL;

        public ProductsController(ILogger<ProductsController> logger, IProductServiceBL productServiceBL)
            : base(logger)
        {
            _productServiceBL = productServiceBL;
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <returns>List of all products.</returns>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Obtener todos los productos",
            Description = "Devuelve una lista de todos los productos disponibles.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _productServiceBL.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products.");
                return StatusCode(500, "An error occurred while retrieving products.");
            }
        }

        /// <summary>
        /// Get a product by its ID.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <returns>Product details.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obtener un producto por ID",
            Description = "Devuelve un producto específico según su ID.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        public async Task<ActionResult<ProductDto>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productServiceBL.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving product with ID {id}.");
                return StatusCode(500, "An error occurred while retrieving the product.");
            }
        }

        /// <summary>
        /// Get a product by its name and size.
        /// </summary>
        /// <param name="name">Product name.</param>
        /// <param name="size">Product size.</param>
        /// <returns>Product details.</returns>
        [HttpGet("name/{name}/size/{size}")]
        [SwaggerOperation(
            Summary = "Obtener un producto por nombre y tamaño",
            Description = "Devuelve un producto específico según su Nombre y Tamaño.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        public async Task<ActionResult<ProductDto>> GetProductByNameAndSizeAsync(string name, string size)
        {
            try
            {
                var product = await _productServiceBL.GetProductByNameAndSizeAsync(name, size);
                if (product == null)
                {
                    return NotFound($"Product with name '{name}' and size '{size}' not found.");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving product with name '{name}' and size '{size}'.");
                return StatusCode(500, "An error occurred while retrieving the product.");
            }
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="product">Product details.</param>
        /// <returns>The created product.</returns>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Crear un nuevo producto",
            Description = "Crea un nuevo producto en el sistema.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was Created successfully.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Something happened while creating the product, please verify the type of the values.")]
        public async Task<ActionResult<ProductDto>> CreateProductAsync([FromBody] Product product)
        {
            try
            {
                var createdProduct = await _productServiceBL.CreateProductAsync(product);
                return CreatedAtAction(nameof(GetProductByIdAsync), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating a new product.");
                return StatusCode(500, "An error occurred while creating the product.");
            }
        }

        /// <summary>
        /// Update product description by product ID.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <param name="newDescription">New description to set.</param>
        /// <returns>No content if successful.</returns>
        [HttpPut("{id}/description")]
        [SwaggerOperation(
            Summary = "Actualizar un producto existente",
            Description = "Actualiza un producto existente según su ID en su Descripcion.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was updated successfully.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Product was not found by their Id or we cannot find the product to update.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Something happened while updating the product, please verify the type of the values.")]
        public async Task<IActionResult> UpdateDescriptionByIdAsync(int id, [FromBody] string newDescription)
        {
            try
            {
                await _productServiceBL.UpdateDescriptionByIdAsync(id, newDescription);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating description for product with ID {id}.");
                return StatusCode(500, "An error occurred while updating the product description.");
            }
        }

        /// <summary>
        /// Update product description by product name and size.
        /// </summary>
        /// <param name="name">Product name.</param>
        /// <param name="size">Product size.</param>
        /// <param name="newDescription">New description to set.</param>
        /// <returns>No content if successful.</returns>
        [HttpPut("name/{name}/size/{size}/description")]
        [SwaggerOperation(
            Summary = "Actualizar un producto existente",
            Description = "Actualiza un producto existente según su Nombre y Tamaño en su Descripcion.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was updated successfully.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Product was not found by their Id or we cannot find the product to update.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Something happened while updating the product, please verify the type of the values.")]
        public async Task<IActionResult> UpdateDescriptionByNameAndSizeAsync(string name, string size, [FromBody] string newDescription)
        {
            try
            {
                await _productServiceBL.UpdateDescriptionByNameAndSizeAsync(name, size, newDescription);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating description for product with name '{name}' and size '{size}'.");
                return StatusCode(500, "An error occurred while updating the product description.");
            }
        }

        /// <summary>
        /// Delete a product by its ID.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Eliminar un producto",
            Description = "Elimina un producto específico según su ID.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was deleted successfully.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Product was not found by their Id.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "The Id param is Invalid.")]
        public async Task<IActionResult> DeleteProductByIdAsync(int id)
        {
            try
            {
                await _productServiceBL.DeleteProductByIdAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID {id}.");
                return StatusCode(500, "An error occurred while deleting the product.");
            }
        }

        /// <summary>
        /// Delete a product by its name and size.
        /// </summary>
        /// <param name="name">Product name.</param>
        /// <param name="size">Product size.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("name/{name}/size/{size}")]
        [SwaggerOperation(
            Summary = "Eliminar un producto",
            Description = "Elimina un producto específico según su Nombre Y Tamaño.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was deleted successfully.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Product was not found by their Id.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "The Id param is Invalid.")]
        public async Task<IActionResult> DeleteProductByNameAndSizeAsync(string name, string size)
        {
            try
            {
                await _productServiceBL.DeleteProductByNameAndSizeAsync(name, size);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with name '{name}' and size '{size}'.");
                return StatusCode(500, "An error occurred while deleting the product.");
            }
        }
    }
}
