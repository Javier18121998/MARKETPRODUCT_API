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

        public async Task<OrderDto> CreateOrderByProductIdAsync(int productId, int quantity)
        {
            try
            {
                var product = await _orderService.CreateOrderByProductIdAsync(productId, quantity);
                return product;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<OrderDto> CreateOrderByProductNameAndSizeAsync(string productName, string productSize, int quantity)
        {
            try
            {
                var productCreated = await _orderService.CreateOrderByProductNameAndSizeAsync(productName, productSize, quantity);
                return productCreated;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteOrderByIdAsync(int id)
        {
            try
            {
                await _orderService.DeleteOrderByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteOrderByProductIdAsync(int productId)
        {
            try
            {
                await _orderService.DeleteOrderByProductIdAsync(productId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteOrderByProductNameAndSizeAsync(string productName, string productSize)
        {
            try
            {
                await _orderService.DeleteOrderByProductNameAndSizeAsync(productName, productSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            try
            {
                return await _orderService.GetAllOrdersAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            try
            {
                var orderObtained = await _orderService.GetOrderByIdAsync(id);
                return orderObtained;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<OrderDto> GetOrderByProductNameAndSizeAsync(string productName, string productSize)
        {
            try
            {
                var orderObtained = await _orderService.GetOrderByProductNameAndSizeAsync(productName, productSize);
                return orderObtained;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateOrderQuantityByIdAsync(int id, int newQuantity)
        {
            try
            {
                await _orderService.UpdateOrderQuantityByIdAsync(id, newQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateOrderQuantityByProductIdAsync(int productId, int newQuantity)
        {
            try
            {
                await _orderService.UpdateOrderQuantityByProductIdAsync(productId, newQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateOrderQuantityByProductNameAndSizeAsync(string productName, string productSize, int newQuantity)
        {
            try
            {
                await _orderService.UpdateOrderQuantityByProductNameAndSizeAsync(productName, productSize, newQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
