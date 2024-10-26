using Market.BL.IBL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Market.DataValidation.DataValidation;
using Market.Exceptions;
using Market.Utilities.BaseControllers;
using Market.Utilities.MQServices.MQModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Market.ProductsController
{
    /// <summary>
    /// Controller for handling product-related operations.
    /// </summary>
    [ApiVersion("1.0")]
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
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "There are no product on the stroe.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Some troubles out of our services was arranged with no expectatioms or too bad.")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogInformation("Requesting all the products");
            try
            {
                #region MQTRansactions Services
                ITransactionIdGenerator transactionIdGenerator = new TransactionIdGenerator();
                var transactionId = transactionIdGenerator.GenerateTransactionId();
                LogMessage logMessage = new LogMessage
                {
                    TransactionId = transactionId,
                    TransactionName = "GetAllProductsAsync",
                    TransactionDescription = "Fetching all available products.",
                    TransactionTime = DateTime.UtcNow,
                    TransactionPathEndpoint = objRequest.ToString()
                };
                #endregion
                var products = await _productServiceBL.GetAllProductsAsync(logMessage);
                _logger.LogInformation($"The products List: {products} were request succesfully.");
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
        [Authorize]
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obtener un producto por ID",
            Description = "Devuelve un producto específico según su ID.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The product not exist or the Id is invalid or was deleted.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Some troubles out of our services was arranged with no expectatioms or too bad.")]
        public async Task<ActionResult<ProductDto>> GetProductByIdAsync(
                [Required]
                [IdValidation]
                int id
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogDebug(id.ToString());
            _logger.LogInformation("Requesting the product by their Id");
            try
            {
                #region MQTRansactions Services
                ITransactionIdGenerator transactionIdGenerator = new TransactionIdGenerator();
                var transactionId = transactionIdGenerator.GenerateTransactionId();
                LogMessage logMessage = new LogMessage
                {
                    TransactionId = transactionId,
                    TransactionName = "GetProductByIdAsync",
                    TransactionDescription = "Fetching product by their Id.",
                    TransactionTime = DateTime.UtcNow,
                    TransactionPathEndpoint = objRequest.ToString()
                };
                #endregion
                var product = await _productServiceBL.GetProductByIdAsync(id, logMessage);
                if (product == null)
                {
                    _logger.LogDebug($"The product not exist");
                    return NotFound($"Product with ID {id} not found.");
                }
                _logger.LogInformation($"The product: {product.ToString()} was requested succesfully");
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
        [Authorize]
        [HttpGet("name/{name}/size/{size}")]
        [SwaggerOperation(
            Summary = "Obtener un producto por nombre y tamaño",
            Description = "Devuelve un producto específico según su Nombre y Tamaño.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The product not exist or the Name and Size are invalid or was deleted.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Some troubles out of our services was arranged with no expectatioms or too bad.")]
        public async Task<ActionResult<ProductDto>> GetProductByNameAndSizeAsync(
                [Required]
                [NameValidation]
                string name,
                [Required]
                [SizeValidation]
                string size
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogDebug($"{name}|{size}");
            _logger.LogInformation("Requesting am product by their Name and Size");
            try
            {
                #region MQTRansactions Services
                ITransactionIdGenerator transactionIdGenerator = new TransactionIdGenerator();
                var transactionId = transactionIdGenerator.GenerateTransactionId();
                LogMessage logMessage = new LogMessage
                {
                    TransactionId = transactionId,
                    TransactionName = "GetProductByNameAndSizeAsync",
                    TransactionDescription = "Fetching a product by their Name and Size.",
                    TransactionTime = DateTime.UtcNow,
                    TransactionPathEndpoint = objRequest.ToString()
                };
                #endregion
                var product = await _productServiceBL.GetProductByNameAndSizeAsync(name, size, logMessage);
                if (product == null)
                {
                    _logger.LogDebug($"The product not exist");
                    return NotFound($"Product with name '{name}' and size '{size}' not found.");
                }
                _logger.LogInformation($"The product: {product.ToString()} was requested succesfully");
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
        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Crear un nuevo producto",
            Description = "Crea un nuevo producto en el sistema.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was Created successfully.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Something happened while creating the product, please verify the type of the values.")]
        public async Task<ActionResult<ProductDto>> CreateProductAsync(
                [FromBody]
                [Required]
                [ProductValidation]
                Product product
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogDebug($"{product.Name}|{product.Price}|{product.Description}1{product.Size}");
            _logger.LogInformation("Creating a new product");
            try
            {
                #region MQTRansactions Services
                ITransactionIdGenerator transactionIdGenerator = new TransactionIdGenerator();
                var transactionId = transactionIdGenerator.GenerateTransactionId();
                LogMessage logMessage = new LogMessage
                {
                    TransactionId = transactionId,
                    TransactionName = "CreateProductAsync",
                    TransactionDescription = "Creating a product.",
                    TransactionTime = DateTime.UtcNow,
                    TransactionPathEndpoint = objRequest.ToString()
                };
                #endregion
                var createdProduct = await _productServiceBL.CreateProductAsync(product, logMessage);
                _logger.LogInformation($"The product: {createdProduct.ToString()} was created succesfully");
                return Created(string.Empty, createdProduct);
            }
            catch (CustomException cex)
            {
                _logger.LogError(cex, "Error creating a new product.");
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error creating a new product.");
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Update product description by product ID.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <param name="newDescription">New description to set.</param>
        /// <returns>No content if successful.</returns>
        [Authorize]
        [HttpPut("{id}/description")]
        [SwaggerOperation(
            Summary = "Actualizar un producto existente",
            Description = "Actualiza un producto existente según su ID en su Descripcion.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was updated successfully.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Product was not found by their Id or we cannot find the product to update.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Something happened while updating the product, please verify the type of the values.")]
        public async Task<IActionResult> UpdateDescriptionByIdAsync(
                [Required]
                [IdValidation]
                int id, 
                [FromBody] 
                [Required]
                [NewDescriptionValidation]
                string newDescription
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogDebug($"{newDescription}");
            _logger.LogInformation("Updating the description into a product by their Id");
            try
            {
                #region MQTRansactions Services
                ITransactionIdGenerator transactionIdGenerator = new TransactionIdGenerator();
                var transactionId = transactionIdGenerator.GenerateTransactionId();
                LogMessage logMessage = new LogMessage
                {
                    TransactionId = transactionId,
                    TransactionName = "UpdateDescriptionByIdAsync",
                    TransactionDescription = "Updating a description by their Id product.",
                    TransactionTime = DateTime.UtcNow,
                    TransactionPathEndpoint = objRequest.ToString()
                };
                #endregion
                await _productServiceBL.UpdateDescriptionByIdAsync(id, newDescription, logMessage);
                _logger.LogInformation("Description into a Product successfully.");
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
        [Authorize]
        [HttpPut("name/{name}/size/{size}/description")]
        [SwaggerOperation(
            Summary = "Actualizar un producto existente",
            Description = "Actualiza un producto existente según su Nombre y Tamaño en su Descripcion.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was updated successfully.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Product was not found by their Id or we cannot find the product to update.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Some params or just one are invalid.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Something happened while updating the product, please verify the type of the values.")]
        public async Task<IActionResult> UpdateDescriptionByNameAndSizeAsync(
                [Required]
                [NameValidation]
                string name,
                [Required]
                [SizeValidation]
                string size, 
                [FromBody]
                [Required]
                [NewDescriptionValidation]
                string newDescription
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogDebug($"{name}|{size}|{newDescription}");
            _logger.LogInformation("Updating the description into a product by their Name and Size");
            try
            {
                #region MQTRansactions Services
                ITransactionIdGenerator transactionIdGenerator = new TransactionIdGenerator();
                var transactionId = transactionIdGenerator.GenerateTransactionId();
                LogMessage logMessage = new LogMessage
                {
                    TransactionId = transactionId,
                    TransactionName = "UpdateDescriptionByNameAndSizeAsync",
                    TransactionDescription = "Updating description by their product name and Size.",
                    TransactionTime = DateTime.UtcNow,
                    TransactionPathEndpoint = objRequest.ToString()
                };
                #endregion
                await _productServiceBL.UpdateDescriptionByNameAndSizeAsync(name, size, newDescription, logMessage);
                _logger.LogInformation("Description into a Product successfully.");
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
        [Authorize]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Eliminar un producto",
            Description = "Elimina un producto específico según su ID.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was deleted successfully.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Product was not found by their Id.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "The Id param is Invalid.")]
        public async Task<IActionResult> DeleteProductByIdAsync(
                [Required]
                [IdValidation]
                int id
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogInformation("Deleting a Product by their Id.");
            try
            {
                #region MQTRansactions Services
                ITransactionIdGenerator transactionIdGenerator = new TransactionIdGenerator();
                var transactionId = transactionIdGenerator.GenerateTransactionId();
                LogMessage logMessage = new LogMessage
                {
                    TransactionId = transactionId,
                    TransactionName = "DeleteProductByIdAsync",
                    TransactionDescription = "Deleting a product by their Id.",
                    TransactionTime = DateTime.UtcNow,
                    TransactionPathEndpoint = objRequest.ToString()
                };
                #endregion
                await _productServiceBL.DeleteProductByIdAsync(id, logMessage);
                _logger.LogInformation("Product was deleted successfully.");
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
        [Authorize]
        [HttpDelete("name/{name}/size/{size}")]
        [SwaggerOperation(
            Summary = "Eliminar un producto",
            Description = "Elimina un producto específico según su Nombre Y Tamaño.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The Product was deleted successfully.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Product was not found by their Id.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "The Id param is Invalid.")]
        public async Task<IActionResult> DeleteProductByNameAndSizeAsync(
                [Required]
                [NameValidation]
                string name,
                [Required]
                [SizeValidation]
                string size
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogDebug($"{name}|{size}");
            _logger.LogInformation("Deleting a Product by their Name and Size.");
            try
            {
                #region MQTRansactions Services
                ITransactionIdGenerator transactionIdGenerator = new TransactionIdGenerator();
                var transactionId = transactionIdGenerator.GenerateTransactionId();
                LogMessage logMessage = new LogMessage
                {
                    TransactionId = transactionId,
                    TransactionName = "DeleteProductByNameAndSizeAsync",
                    TransactionDescription = "Deleting a product by their Name and Size.",
                    TransactionTime = DateTime.UtcNow,
                    TransactionPathEndpoint = objRequest.ToString()
                };
                #endregion
                await _productServiceBL.DeleteProductByNameAndSizeAsync(name, size, logMessage);
                _logger.LogInformation("Product was deleted successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with name '{name}' and size '{size}'.");
                return StatusCode(500, "An error occurred while deleting the product.");
            }
        }
    }

    public interface ITransactionIdGenerator
    {
        short GenerateTransactionId();
    }
    public class TransactionIdGenerator : ITransactionIdGenerator
    {
        private static short _currentId;
        public short GenerateTransactionId() => ++_currentId;
    }
}
