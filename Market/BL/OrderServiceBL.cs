using Market.BL.IBL;
using Market.DAL.IDAL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.BL
{
    public class OrderServiceBL : IOrderServiceBL
    {
        private readonly IOrderService _orderService;

        public OrderServiceBL(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Creates a new order based on the product ID.
        /// </summary>
        /// <param name="productId">The ID of the product to order.</param>
        /// <param name="quantity">The quantity of the product to order.</param>
        /// <returns>The created order DTO.</returns>
        public async Task<OrderDto> CreateOrderByProductIdAsync(int productId, int quantity)
        {
            try
            {
                var product = await _orderService.CreateOrderByProductIdAsync(productId, quantity);
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new order based on the product name and size.
        /// </summary>
        /// <param name="productName">The name of the product to order.</param>
        /// <param name="productSize">The size of the product to order.</param>
        /// <param name="quantity">The quantity of the product to order.</param>
        /// <returns>The created order DTO.</returns>
        public async Task<OrderDto> CreateOrderByProductNameAndSizeAsync(string productName, string productSize, int quantity)
        {
            try
            {
                var productCreated = await _orderService.CreateOrderByProductNameAndSizeAsync(productName, productSize, quantity);
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                return productCreated;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        public async Task DeleteOrderByIdAsync(int id)
        {
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                await _orderService.DeleteOrderByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an order by the product ID.
        /// </summary>
        /// <param name="productId">The ID of the product associated with the order to delete.</param>
        public async Task DeleteOrderByProductIdAsync(int productId)
        {
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                await _orderService.DeleteOrderByProductIdAsync(productId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an order by the product name and size.
        /// </summary>
        /// <param name="productName">The name of the product associated with the order to delete.</param>
        /// <param name="productSize">The size of the product associated with the order to delete.</param>
        public async Task DeleteOrderByProductNameAndSizeAsync(string productName, string productSize)
        {
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                await _orderService.DeleteOrderByProductNameAndSizeAsync(productName, productSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <returns>A list of order DTOs.</returns>
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                return await _orderService.GetAllOrdersAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves an order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The corresponding order DTO.</returns>
        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            try
            {
                var orderObtained = await _orderService.GetOrderByIdAsync(id);
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                return orderObtained;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves an order by the product name and size.
        /// </summary>
        /// <param name="productName">The name of the product associated with the order.</param>
        /// <param name="productSize">The size of the product associated with the order.</param>
        /// <returns>The corresponding order DTO.</returns>
        public async Task<OrderDto> GetOrderByProductNameAndSizeAsync(string productName, string productSize)
        {
            try
            {
                var orderObtained = await _orderService.GetOrderByProductNameAndSizeAsync(productName, productSize);
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                return orderObtained;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates the quantity of an order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to update.</param>
        /// <param name="newQuantity">The new quantity for the order.</param>
        public async Task UpdateOrderQuantityByIdAsync(int id, int newQuantity)
        {
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                await _orderService.UpdateOrderQuantityByIdAsync(id, newQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates the quantity of an order by the product ID.
        /// </summary>
        /// <param name="productId">The ID of the product associated with the order to update.</param>
        /// <param name="newQuantity">The new quantity for the order.</param>
        public async Task UpdateOrderQuantityByProductIdAsync(int productId, int newQuantity)
        {
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                await _orderService.UpdateOrderQuantityByProductIdAsync(productId, newQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates the quantity of an order by the product name and size.
        /// </summary>
        /// <param name="productName">The name of the product associated with the order to update.</param>
        /// <param name="productSize">The size of the product associated with the order to update.</param>
        /// <param name="newQuantity">The new quantity for the order.</param>
        public async Task UpdateOrderQuantityByProductNameAndSizeAsync(string productName, string productSize, int newQuantity)
        {
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                await _orderService.UpdateOrderQuantityByProductNameAndSizeAsync(productName, productSize, newQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
