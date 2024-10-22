using MARKETPRODUCT_API.Data.DTOs;
using Microsoft.Extensions.Logging;

namespace MARKETPRODUCT_API.Data.ModelValidations
{
    public class OrderModelValidations : IOrderModelValidations
    {
        private readonly ILogger<OrderModelValidations> _logger;

        public OrderModelValidations(ILogger<OrderModelValidations> logger)
        {
            _logger = logger;
        }

        public bool ValidateOrder(CreateOrderDto orderDto)
        {
            if (orderDto == null || orderDto.Items == null || !orderDto.Items.Any())
            {
                _logger.LogWarning("OrderDto is null or has no items.");
                return false;
            }

            foreach (var item in orderDto.Items)
            {
                if (!ValidateOrderItem(item))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidateOrderItem(CreateOrderItemDto item)
        {
            if (item.ProductId <= 0)
            {
                _logger.LogWarning($"Invalid ProductId: {item.ProductId}. It must be greater than 0.");
                return false;
            }

            if (item.Quantity <= 0)
            {
                _logger.LogWarning($"Invalid Quantity: {item.Quantity}. It must be greater than 0.");
                return false;
            }

            return true;
        }

        public bool ValidateOrderId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning($"Invalid order ID: {id}. It must be greater than 0.");
                return false;
            }
            return true;
        }
    }
}
