using MARKETPRODUCT_API.Data.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks; 

namespace MARKETPRODUCT_API.Services.IServices
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto); 
        Task<OrderDto> GetOrderByIdAsync(int id); 
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(); 
        Task DeleteOrderAsync(int id); 
    }
}
