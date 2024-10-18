using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DAL.IDAL
{
    public interface IOrderService
    {
        // Crear una nueva orden por medio del nombre y tamaño del producto
        Task<OrderDto> CreateOrderByProductNameAndSizeAsync(string productName, string productSize, int quantity);
        // Crear una nueva orden por ID del producto
        Task<OrderDto> CreateOrderByProductIdAsync(int productId, int quantity);

        // Actualizar cantidad de una orden por ID de la orden
        Task UpdateOrderQuantityByIdAsync(int id, int newQuantity);

        // Actualizar cantidad de una orden por ID del producto
        Task UpdateOrderQuantityByProductIdAsync(int productId, int newQuantity);

        // Actualizar cantidad de una orden por nombre y tamaño del producto
        Task UpdateOrderQuantityByProductNameAndSizeAsync(string productName, string productSize, int newQuantity);

        // Eliminar una orden por ID
        Task DeleteOrderByIdAsync(int id);

        // Eliminar una orden por ID del producto
        Task DeleteOrderByProductIdAsync(int productId);

        // Eliminar una orden por nombre y tamaño del producto
        Task DeleteOrderByProductNameAndSizeAsync(string productName, string productSize);

        // Obtener todas las órdenes
        Task<List<OrderDto>> GetAllOrdersAsync();

        // Obtener una orden por ID
        Task<OrderDto> GetOrderByIdAsync(int id);

        // Obtener una orden por nombre y tamaño del producto
        Task<OrderDto> GetOrderByProductNameAndSizeAsync(string productName, string productSize);
    }
}
