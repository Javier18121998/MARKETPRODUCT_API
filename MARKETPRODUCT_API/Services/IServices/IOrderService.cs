using MARKETPRODUCT_API.Data.DTOs;

namespace MARKETPRODUCT_API.Services.IServices
{
    public interface IOrderService
    {
        OrderDto CreateOrder(CreateOrderDto createOrderDto);
        OrderDto GetOrderById(int id);
        IEnumerable<OrderDto> GetAllOrders();
        void DeleteOrder(int id);
    }
}
