using MARKETPRODUCT_API.Data.EFModels;
using MARKETPRODUCT_API.Data;
using Microsoft.AspNetCore.Mvc;
using MARKETPRODUCT_API.Messaging.MessageProducer;
using MARKETPRODUCT_API.Messaging.MessageModels;
using MARKETPRODUCT_API.Services.IServices;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;
using System.Net;
using MARKETPRODUCT_API.Controllers.Utilities;
using MARKETPRODUCT_API.Data.ModelValidations.IDataModelValidations;
using Microsoft.AspNetCore.Authorization;

namespace MARKETPRODUCT_API.Controllers
{
    /// <summary>
    /// Purpose: Manage the Products through API endpoints
    /// </summary>
    public class ProductController : MarketProductControllerBase<ProductController>
    {
        private readonly IProductService _productService;
        private readonly IProductModelValidations _productModelValidations;

        public ProductController(IProductService productService, ILogger<ProductController> logger, IProductModelValidations productModelValidations)
            : base(logger)
        {
            _productService = productService;
            _productModelValidations = productModelValidations;
        }

        /// <summary>
        /// Fetch all available products.
        /// </summary>
        /// <returns>A list of all products.</returns>
        [Authorize]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Obtener todos los productos",
            Description = "Devuelve una lista de todos los productos disponibles.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogInformation("Obteniendo todos los productos.");
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Obtain a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The product matching the specified ID.</returns>
        [Authorize]
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obtener un producto por ID",
            Description = "Devuelve un producto específico según su ID.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogInformation($"Obteniendo producto con ID {id}.");
            #region Approach to Model-Validations
            if (!_productModelValidations.ValidateProductId(id))
            {
                return BadRequest("Product ID must be greater than 0.");
            }
            #endregion
            var product = await _productService.GetProductAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Producto con ID {id} no encontrado.");
                return NotFound();
            }
            return Ok(product);
        }

        /// <summary>
        /// Create a new product in the system.
        /// </summary>
        /// <param name="newProduct">The product details to create.</param>
        /// <returns>The created product.</returns>
        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Crear un nuevo producto",
            Description = "Crea un nuevo producto en el sistema.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was Created successfully.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Something happened while creating the product, please verify the type of the values.")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product newProduct)
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogDebug($"{newProduct.Name}||{newProduct.Price}");
            #region Approach to Model-Validations
            if (newProduct == null ||
                !_productModelValidations.ValidateProductName(newProduct.Name) ||
                !_productModelValidations.ValidateProductPrice(newProduct.Price))
            {
                return BadRequest("Invalid product details.");
            }
            #endregion
            if (newProduct == null)
            {
                _logger.LogError("El producto a crear es nulo.");
                return BadRequest();
            }

            var createdProduct = await _productService.CreateProductAsync(newProduct);
            _logger.LogInformation($"Producto con ID {createdProduct.Id} creado.");
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        /// <summary>
        /// Update an existing product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="updatedProduct">The updated product details.</param>
        /// <returns>No content if the update was successful.</returns>
        [Authorize]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Actualizar un producto existente",
            Description = "Actualiza un producto existente según su ID.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was updated successfully.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Product was not found by their Id or we cannot find the product to update.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Something happened while updating the product, please verify the type of the values.")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogDebug($"{updatedProduct.Name}||{updatedProduct.Price}");
            try
            {
                #region Approach to Model-Validations
                if (!_productModelValidations.ValidateProductId(id))
                {
                    return BadRequest("Product ID must be greater than 0.");
                }
                if (updatedProduct == null ||
                    !_productModelValidations.ValidateProductName(updatedProduct.Name) ||
                    !_productModelValidations.ValidateProductPrice(updatedProduct.Price))
                {
                    return BadRequest("Invalid product details.");
                }
                #endregion
                _logger.LogInformation($"Actualizando producto con ID {id}.");
                await _productService.UpdateProductAsync(id, updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el producto con ID {id}.");
                return NotFound(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>No content if the deletion was successful.</returns>
        [Authorize]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Eliminar un producto por ID",
            Description = "Elimina un producto específico según su ID.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was deleted successfully.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Product was not found by their Id.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "The Id param is Invalid.")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogDebug(id.ToString());
            try
            {
                #region Approach to Model-Validations
                if (!_productModelValidations.ValidateProductId(id))
                {
                    return BadRequest("Product ID must be greater than 0.");
                }
                #endregion
                _logger.LogInformation($"Eliminando producto con ID {id}.");
                await _productService.DeleteProductAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el producto con ID {id}.");
                return NotFound(ex.Message);
            }

            return NoContent();
        }
    }
}
