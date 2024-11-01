using Market.DAL.IDAL;
using Market.Utilities.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;

namespace Market.CartsController
{
    [ApiVersion("2.0")]
    public class CartsController : MarketProductControllerBase<CartsController>
    {
        private readonly ICartService _cartService;

        public CartsController(ILogger<CartsController> logger, ICartService cartService)
            : base(logger)
        {
            _cartService = cartService;
        }

        [HttpPost("AddItem")]
        [SwaggerOperation(
            Summary = "Añade un producto al carrito del cliente",
            Description = "Permite agregar un producto al carrito del cliente mediante su sesión activa.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Producto añadido correctamente.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Error al añadir el producto.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error de servidor.")]
        public async Task<ActionResult> AddItemToCart([FromBody] CartItemRequest cartItemRequest)
        {
            try
            {
                var result = await _cartService.AddItemToCartAsync(cartItemRequest.ProductName, cartItemRequest.Quantity, cartItemRequest.Size);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al añadir producto al carrito.");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Operación inválida al añadir producto al carrito.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al añadir producto al carrito.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error al añadir producto al carrito.");
            }
        }

        [HttpGet("current")]
        [SwaggerOperation(
            Summary = "Obtiene el carrito actual del cliente",
            Description = "Devuelve el contenido del carrito actual del cliente.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Carrito obtenido correctamente.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error de servidor.")]
        public async Task<ActionResult> GetCurrentCart()
        {
            try
            {
                var cart = await _cartService.GetCustomerCartAsync();
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el carrito.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error al obtener el carrito.");
            }
        }

        [HttpDelete("remove-item")]
        [SwaggerOperation(
            Summary = "Elimina un producto del carrito",
            Description = "Permite eliminar un producto específico del carrito del cliente.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Producto eliminado correctamente.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Error al eliminar el producto.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error de servidor.")]
        public async Task<ActionResult> RemoveItemFromCart([FromBody] RemoveCartItemRequest removeRequest)
        {
            try
            {
                bool success = await _cartService.RemoveItemFromCartAsync(removeRequest.ProductName, removeRequest.Size);

                if (!success)
                    return BadRequest("El producto no se encontró en el carrito o hubo un problema al eliminarlo.");

                return Ok("Producto eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto del carrito.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error al eliminar el producto del carrito.");
            }
        }
    }
}
