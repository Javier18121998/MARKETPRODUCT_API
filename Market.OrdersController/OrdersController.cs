using Market.BL.IBL;
using Market.DataModels.DTos;
using Market.Utilities.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Market.OrdersController
{
    public class OrdersController : MarketProductControllerBase<OrdersController>
    {
        private readonly IOrderServiceBL _orderServiceBL;

        public OrdersController(ILogger<OrdersController> logger, IOrderServiceBL orderServiceBL)
            : base(logger)
        {
            _orderServiceBL = orderServiceBL;
        }

        // 1. Crear una nueva orden por medio del nombre y tamaño del producto
        [HttpPost("CreateByNameAndSize")]
        [SwaggerOperation(
            Summary = "Crear una Orden",
            Description = "Crear una Orden por medio del Nombre del Producto y su Tamaño")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Producto No encontrado Falló el Nombre y/o el Tamaño.")]
        public async Task<IActionResult> CreateOrderByProductNameAndSize([FromBody] CreateOrderByProductNameAndSizeDto request)
        {
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

        // 2. Crear una nueva orden por ID del producto
        [HttpPost("CreateById")]
        [SwaggerOperation(
            Summary = "Crear una Orden",
            Description = "Crear una Orden por medio del Id del Producto")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Producto No encontrado Falló el Id del Producto.")]
        public async Task<IActionResult> CreateOrderByProductId([FromBody] CreateOrderByProductIdDto request)
        {
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

        // 3. Actualizar cantidad de una orden por ID de la orden
        [HttpPut("UpdateQuantityById/{id}")]
        [SwaggerOperation(
            Summary = "Modificar una Orden",
            Description = "Modificar una Orden por medio del Id de la Orden")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Id de la Orden.")]
        public async Task<IActionResult> UpdateOrderQuantityById(int id, [FromBody] UpdateOrderQuantityDto request)
        {
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

        // 4. Actualizar cantidad de una orden por ID del producto
        [HttpPut("UpdateQuantityByProductId/{productId}")]
        [SwaggerOperation(
            Summary = "Modificar una Orden",
            Description = "Modificar una Orden por medio del Id del producto")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Id del producto.")]
        public async Task<IActionResult> UpdateOrderQuantityByProductId(int productId, [FromBody] UpdateOrderQuantityDto request)
        {
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

        // 5. Actualizar cantidad de una orden por nombre y tamaño del producto
        [HttpPut("UpdateQuantityByNameAndSize")]
        [SwaggerOperation(
            Summary = "Modificar una Orden",
            Description = "Modificar una Orden por medio del Nombre del Producto y El tamaño")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Nombre del Producto y/o el Tamaño.")]
        public async Task<IActionResult> UpdateOrderQuantityByProductNameAndSize([FromBody] UpdateOrderQuantityByNameAndSizeDto request)
        {
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

        // 6. Eliminar una orden por ID
        [HttpDelete("DeleteById/{id}")]
        [SwaggerOperation(
            Summary = "Eliminar una Orden.",
            Description = "Eliminar una Orden por su Id.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Id de la Orden.")]
        public async Task<IActionResult> DeleteOrderById(int id)
        {
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

        // 7. Eliminar una orden por ID del producto
        [HttpDelete("DeleteByProductId/{productId}")]
        [SwaggerOperation(
            Summary = "Eliminar una Orden.",
            Description = "Eliminar una Orden.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Id del Producto")]
        public async Task<IActionResult> DeleteOrderByProductId(int productId)
        {
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

        // 8. Eliminar una orden por nombre y tamaño del producto
        [HttpDelete("DeleteByNameAndSize")]
        [SwaggerOperation(
            Summary = "Eliminar una Orden.",
            Description = "Eliminar una Orden por medio del Nombre del Producto y su tamño.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Nombre del Producto y/o el Tamaño.")]
        public async Task<IActionResult> DeleteOrderByProductNameAndSize([FromBody] DeleteOrderByNameAndSizeDto request)
        {
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

        // 9. Obtener todas las órdenes
        [HttpGet("GetAll")]
        [SwaggerOperation(
            Summary = "Obtener todas las Ordenes.",
            Description = "Obtener todas las Ordenes.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "No hay Ordenes.")]
        public async Task<IActionResult> GetAllOrders()
        {
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

        // 10. Obtener una orden por ID
        [HttpGet("GetById/{id}")]
        [SwaggerOperation(
            Summary = "Obtener una Orden",
            Description = "Obtener una Orden por medio del Id de la Orden.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Id de la Orden.")]
        public async Task<IActionResult> GetOrderById(int id)
        {
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

        // 11. Obtener una orden por nombre y tamaño del producto
        [HttpGet("GetByNameAndSize")]
        [SwaggerOperation(
            Summary = "Obtener una Orden",
            Description = "Obtener una Orden por medio del Nombre del Producto y El tamaño")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Orden no encontrada Falló el Nombre del Producto y/o el Tamaño.")]
        public async Task<IActionResult> GetOrderByProductNameAndSize([FromQuery] string productName, [FromQuery] string productSize)
        {
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
