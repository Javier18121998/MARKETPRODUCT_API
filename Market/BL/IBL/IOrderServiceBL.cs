using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.BL.IBL
{
    public interface IOrderServiceBL
    {
        /// <summary>
        /// Creates a new order by the product's name and size.
        /// </summary>
        /// <param name="productName">The name of the product.</param>
        /// <param name="productSize">The size of the product.</param>
        /// <param name="quantity">The quantity of the product.</param>
        /// <returns>The created order.</returns>
        Task<OrderDto> CreateOrderByProductNameAndSizeAsync(string productName, string productSize, int quantity);

        /// <summary>
        /// Creates a new order by the product ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="quantity">The quantity of the product.</param>
        /// <returns>The created order.</returns>
        Task<OrderDto> CreateOrderByProductIdAsync(int productId, int quantity);

        /// <summary>
        /// Updates the quantity of an order by the order ID.
        /// </summary>
        /// <param name="id">The ID of the order to update.</param>
        /// <param name="newQuantity">The new quantity for the order.</param>
        /// <returns></returns>
        Task UpdateOrderQuantityByIdAsync(int id, int newQuantity);

        /// <summary>
        /// Updates the quantity of an order by the product ID.
        /// </summary>
        /// <param name="productId">The ID of the product associated with the order.</param>
        /// <param name="newQuantity">The new quantity for the order.</param>
        /// <returns></returns>
        Task UpdateOrderQuantityByProductIdAsync(int productId, int newQuantity);

        /// <summary>
        /// Updates the quantity of an order by the product's name and size.
        /// </summary>
        /// <param name="productName">The name of the product.</param>
        /// <param name="productSize">The size of the product.</param>
        /// <param name="newQuantity">The new quantity for the order.</param>
        /// <returns></returns>
        Task UpdateOrderQuantityByProductNameAndSizeAsync(string productName, string productSize, int newQuantity);

        /// <summary>
        /// Deletes an order by the order ID.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns></returns>
        Task DeleteOrderByIdAsync(int id);

        /// <summary>
        /// Deletes an order by the product ID.
        /// </summary>
        /// <param name="productId">The ID of the product associated with the order.</param>
        /// <returns></returns>
        Task DeleteOrderByProductIdAsync(int productId);

        /// <summary>
        /// Deletes an order by the product's name and size.
        /// </summary>
        /// <param name="productName">The name of the product.</param>
        /// <param name="productSize">The size of the product.</param>
        /// <returns></returns>
        Task DeleteOrderByProductNameAndSizeAsync(string productName, string productSize);

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <returns>A list of all orders.</returns>
        Task<List<OrderDto>> GetAllOrdersAsync();

        /// <summary>
        /// Retrieves an order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The corresponding order.</returns>
        Task<OrderDto> GetOrderByIdAsync(int id);

        /// <summary>
        /// Retrieves an order by the product's name and size.
        /// </summary>
        /// <param name="productName">The name of the product.</param>
        /// <param name="productSize">The size of the product.</param>
        /// <returns>The corresponding order.</returns>
        Task<OrderDto> GetOrderByProductNameAndSizeAsync(string productName, string productSize);
    }
}
