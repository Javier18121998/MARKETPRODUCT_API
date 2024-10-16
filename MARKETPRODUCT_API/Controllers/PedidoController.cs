using MARKETPRODUCT_API.Data;
using MARKETPRODUCT_API.Data.DTOs;
using MARKETPRODUCT_API.Data.EFModels;
using MARKETPRODUCT_API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MARKETPRODUCT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public PedidoController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<OrderDto>> GetPedidos()
        {
            return Ok(_orderService.GetAllOrders());
        }

        [HttpGet("{id}")]
        public ActionResult<OrderDto> GetPedido(int id)
        {
            try
            {
                return Ok(_orderService.GetOrderById(id));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<OrderDto> CreatePedido([FromBody] CreateOrderDto orderDto)
        {
            try
            {
                var order = _orderService.CreateOrder(orderDto);
                return CreatedAtAction(nameof(GetPedido), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePedido(int id)
        {
            try
            {
                _orderService.DeleteOrder(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
