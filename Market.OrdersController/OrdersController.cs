using Market.BL.IBL;
using Market.DataModels.DTos;
using Market.DataValidation.DataValidation;
using Market.Utilities.BaseControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Market.OrdersController
{
    /// <summary>
    /// Controll all the orders and Manage their behaviors via MarketProductControllerBase route
    /// </summary>
    public class OrdersController : MarketProductControllerBase<OrdersController>
    {
        private readonly IOrderServiceBL _orderServiceBL;

        public OrdersController(ILogger<OrdersController> logger, IOrderServiceBL orderServiceBL)
            : base(logger)
        {
            _orderServiceBL = orderServiceBL;
        }

        /// <summary>
        /// 1. Create an Order using the Name and Size of the product.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("CreateByNameAndSize")]
        [SwaggerOperation(
            Summary = "Crear una Orden",
            Description = "Crear una Orden por medio del Nombre del Producto y su Tamaño")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Producto No encontrado Falló el Nombre y/o el Tamaño.")]
        public async Task<IActionResult> CreateOrderByProductNameAndSize(
            [FromBody]
            [CreateOrderByProductNameAndSizeValidation]
            CreateOrderByProductNameAndSizeDto request)
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            try
            {
                var order = await _orderServiceBL.CreateOrderByProductNameAndSizeAsync(request.ProductName, request.ProductSize, request.Quantity);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una orden por nombre y tamaño del producto");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 2. Create an Order via the Id of the product.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("CreateById")]
        [SwaggerOperation(
            Summary = "Crear una Orden",
            Description = "Crear una Orden por medio del Id del Producto")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Producto No encontrado Falló el Id del Producto.")]
        public async Task<IActionResult> CreateOrderByProductId(
                [FromBody]
                [CreateOrderByProductIdValidation]
                CreateOrderByProductIdDto request
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            try
            {
                var order = await _orderServiceBL.CreateOrderByProductIdAsync(request.ProductId, request.Quantity);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una orden por ID del producto");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 3. Modify the Order with Id Order.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("UpdateQuantityById/{id}")]
        [SwaggerOperation(
            Summary = "Modificar una Orden",
            Description = "Modificar una Orden por medio del Id de la Orden")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Id de la Orden.")]
        public async Task<IActionResult> UpdateOrderQuantityById(
                [Required]
                [IdValidation]
                int id, 
                [FromBody]
                [UpdateOrderQuantityValidation]
                DataModels.DTos.UpdateOrderQuantityDto request
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            try
            {
                await _orderServiceBL.UpdateOrderQuantityByIdAsync(id, request.NewQuantity);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la cantidad de la orden por ID");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 4. Update the order via ProductId in their Quaantity.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("UpdateQuantityByProductId/{productId}")]
        [SwaggerOperation(
            Summary = "Modificar una Orden",
            Description = "Modificar una Orden por medio del Id del producto")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Id del producto.")]
        public async Task<IActionResult> UpdateOrderQuantityByProductId(
                [Required]
                [IdValidation]
                int productId, 
                [FromBody]
                [UpdateOrderQuantityValidation]
                DataModels.DTos.UpdateOrderQuantityDto request
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            try
            {
                await _orderServiceBL.UpdateOrderQuantityByProductIdAsync(productId, request.NewQuantity);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la cantidad de la orden por ID del producto");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 5. Modify the Order via Name of the PRoduct and Product Size in their Quantity.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("UpdateQuantityByNameAndSize")]
        [SwaggerOperation(
            Summary = "Modificar una Orden",
            Description = "Modificar una Orden por medio del Nombre del Producto y El tamaño")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Nombre del Producto y/o el Tamaño.")]
        public async Task<IActionResult> UpdateOrderQuantityByProductNameAndSize(
                [FromBody]
                [UpdateOrderQuantityByNameAndSizeValidation]
                UpdateOrderQuantityByNameAndSizeDto request
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            try
            {
                await _orderServiceBL.UpdateOrderQuantityByProductNameAndSizeAsync(request.ProductName, request.ProductSize, request.NewQuantity);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la cantidad de la orden por nombre y tamaño del producto");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 6. Delete an Order via their Id Order.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("DeleteById/{id}")]
        [SwaggerOperation(
            Summary = "Eliminar una Orden.",
            Description = "Eliminar una Orden por su Id.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Id de la Orden.")]
        public async Task<IActionResult> DeleteOrderById(
                [Required]
                [IdValidation]
                int id
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            try
            {
                await _orderServiceBL.DeleteOrderByIdAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la orden por ID");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 7. Dismisnis an Order via their ProductId.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("DeleteByProductId/{productId}")]
        [SwaggerOperation(
            Summary = "Eliminar una Orden.",
            Description = "Eliminar una Orden.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Id del Producto")]
        public async Task<IActionResult> DeleteOrderByProductId(int productId)
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            try
            {
                await _orderServiceBL.DeleteOrderByProductIdAsync(productId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la orden por ID del producto");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 8. Delete an Order via Product Name and their Size.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("DeleteByNameAndSize")]
        [SwaggerOperation(
            Summary = "Eliminar una Orden.",
            Description = "Eliminar una Orden por medio del Nombre del Producto y su tamño.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Nombre del Producto y/o el Tamaño.")]
        public async Task<IActionResult> DeleteOrderByProductNameAndSize(
                [FromBody]
                [DeleteOrderByNameAndSizeValidation]
                DeleteOrderByNameAndSizeDto request
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            try
            {
                await _orderServiceBL.DeleteOrderByProductNameAndSizeAsync(request.ProductName, request.ProductSize);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la orden por nombre y tamaño del producto");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 9. Get all the Orders.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetAll")]
        [SwaggerOperation(
            Summary = "Obtener todas las Ordenes.",
            Description = "Obtener todas las Ordenes.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "No hay Ordenes.")]
        public async Task<IActionResult> GetAllOrders()
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            try
            {
                var orders = await _orderServiceBL.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las órdenes");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 10. Obtain an Order via Id Order.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetById/{id}")]
        [SwaggerOperation(
            Summary = "Obtener una Orden",
            Description = "Obtener una Orden por medio del Id de la Orden.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Id de la Orden.")]
        public async Task<IActionResult> GetOrderById(
                [Required]
                [IdValidation]
                int id
            )
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            try
            {
                var order = await _orderServiceBL.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound($"Orden con ID {id} no encontrada");
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la orden por ID");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 11. Obtain an Order via Product Name and Size.
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="productSize"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetByNameAndSize")]
        [SwaggerOperation(
            Summary = "Obtener una Orden",
            Description = "Obtener una Orden por medio del Nombre del Producto y El tamaño")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Nombre del Producto y/o el Tamaño.")]
        public async Task<IActionResult> GetOrderByProductNameAndSize(
                [FromQuery]
                [Required]
                [NameValidation]
                string productName, 
                [FromQuery]
                [Required]
                [SizeValidation]
                string productSize)
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            try
            {
                var order = await _orderServiceBL.GetOrderByProductNameAndSizeAsync(productName, productSize);
                if (order == null)
                {
                    return NotFound($"Orden con nombre '{productName}' y tamaño '{productSize}' no encontrada");
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la orden por nombre y tamaño del producto");
                return BadRequest(ex.Message);
            }
        }
    }
}
