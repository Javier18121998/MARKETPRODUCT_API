using MARKETPRODUCT_API.Data;
using MARKETPRODUCT_API.Data.DTOs;
using MARKETPRODUCT_API.Data.EFModels;
using MARKETPRODUCT_API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using MARKETPRODUCT_API.Controllers.Utilities;
using MARKETPRODUCT_API.Data.ModelValidations;

namespace MARKETPRODUCT_API.Controllers
{
    /// <summary>
    /// Purpose: Manage orders through API endpoints
    /// </summary>
    public class PedidoController : MarketProductControllerBase<PedidoController>
    {
        private readonly IOrderService _orderService;
        private readonly IOrderModelValidations _orderModelValidations;

        public PedidoController(IOrderService orderService, ILogger<PedidoController> logger, IOrderModelValidations orderModelValidations)
            : base(logger)
        {
            _orderService = orderService;
            _orderModelValidations = orderModelValidations;
        }

        /// <summary>
        /// Fetch all orders made.
        /// </summary>
        /// <returns>A list of all orders.</returns>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Obtener todos los pedidos",
            Description = "Devuelve una lista de todos los pedidos realizados.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Order lists are empty or not found.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "The system caught a non-observable event.")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetPedidos()
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogInformation("Obteniendo todos los pedidos.");
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Obtain a specific order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The order matching the specified ID.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obtener un pedido por ID",
            Description = "Devuelve un pedido específico según su ID.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Order List does not exist or the ID for this Order is wrong.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "The system caught a non-observable event.")]
        public async Task<ActionResult<OrderDto>> GetPedido(int id)
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogDebug(id.ToString());
            try
            {
                #region Approach to Model-Validations
                if (!_orderModelValidations.ValidateOrderId(id))
                {
                    return BadRequest("Invalid order ID.");
                }
                #endregion
                _logger.LogInformation($"Obteniendo pedido con ID {id}.");
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error al obtener el pedido con ID {id}.");
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Create a new order with the provided details.
        /// </summary>
        /// <param name="orderDto">The details of the order to create.</param>
        /// <returns>The created order.</returns>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Crear un nuevo pedido",
            Description = "Crea un nuevo pedido con los detalles proporcionados.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "The system caught a non-observable event.")]
        public async Task<ActionResult<OrderDto>> CreatePedido([FromBody] CreateOrderDto orderDto)
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            try
            {
                #region Approach to Model-Validations
                if (!_orderModelValidations.ValidateOrder(orderDto))
                {
                    return BadRequest("Invalid order details.");
                }
                #endregion
                _logger.LogInformation("Creando un nuevo pedido.");
                var order = await _orderService.CreateOrderAsync(orderDto);
                return CreatedAtAction(nameof(GetPedido), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el pedido.");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a specific order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns>No content if the deletion was successful.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Eliminar un pedido por ID",
            Description = "Elimina un pedido específico según su ID.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Order does not exist or the ID for this Order is wrong.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "The system caught a non-observable event.")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var objRequest = $"{HttpContext.Request.Method}/{HttpContext.Request.Host}/{HttpContext.Request.Path}/{HttpContext.Request.QueryString}";
            _logger.LogDebug(objRequest.ToString());
            _logger.LogDebug(id.ToString());
            try
            {
                #region Approach to Model-Validations
                if (!_orderModelValidations.ValidateOrderId(id))
                {
                    return BadRequest("Invalid order ID.");
                }
                #endregion
                _logger.LogInformation($"Eliminando pedido con ID {id}.");
                await _orderService.DeleteOrderAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el pedido con ID {id}.");
                return NotFound(ex.Message);
            }
        }

    }
}
