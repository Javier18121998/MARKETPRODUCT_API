using MARKETPRODUCT_API.Data.DTOs;

namespace MARKETPRODUCT_API.Data.ModelValidations
{
    public interface IOrderModelValidations
    {
        bool ValidateOrder(CreateOrderDto orderDto);
        bool ValidateOrderItem(CreateOrderItemDto item);
        bool ValidateOrderId(int id);
    }
}
