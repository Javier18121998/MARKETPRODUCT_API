using Market.DAL.IDAL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Market.DataValidation.IDataBaseValidations;
using Market.Exceptions;
using Market.Market.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Market.DAL
{
    /// <summary>
    /// Produce the service Data Acces LAyer into Orders Management.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly MarketDbContext _context;
        private readonly IOrderValidationService _orderValidationService;

        public OrderService(MarketDbContext context, IOrderValidationService orderValidationService)
        {
            _context = context;
            _orderValidationService = orderValidationService;
        }

        /// <summary>
        /// Creates a new order based on the product name and size.
        /// </summary>
        /// <param name="productName">The name of the product.</param>
        /// <param name="productSize">The size of the product.</param>
        /// <param name="quantity">The quantity to order.</param>
        /// <returns>The created order.</returns>
        public async Task<OrderDto> CreateOrderByProductNameAndSizeAsync(
            string productName,
            string productSize,
            int quantity)
        {
            var product = await _context.Products.FirstOrDefaultAsync(
                p => p.ProductName == productName &&
                p.ProductSize == productSize
            );

            if (product == null)
            {
                throw new CustomException(HttpStatusCode.BadRequest, "The product with the specified name and size does not exist.");
            }

            var order = new OrderDto
            {
                ProductId = product.Id,
                Quantity = quantity,
                CreateOrder = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        /// <summary>
        /// Creates a new order based on the product ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="quantity">The quantity to order.</param>
        /// <returns>The created order.</returns>
        public async Task<OrderDto> CreateOrderByProductIdAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new CustomException(HttpStatusCode.BadRequest, "The product with the specified ID does not exist.");
            }

            var order = new OrderDto
            {
                ProductId = productId,
                Quantity = quantity,
                CreateOrder = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        /// <summary>
        /// Updates the quantity of an order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <param name="newQuantity">The new quantity.</param>
        /// <returns></returns>
        public async Task UpdateOrderQuantityByIdAsync(int id, int newQuantity)
        {
            if (await _orderValidationService.OrderExistsByIdAsync(id))
            {
                var order = await _context.Orders.FindAsync(id);
                if (order != null)
                {
                    order.Quantity = newQuantity;
                    order.CreateOrder = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                throw new CustomException(HttpStatusCode.BadRequest, "The order with the specified ID does not exist.");
            }
        }

        /// <summary>
        /// Updates the quantity of an order by the product ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="newQuantity">The new quantity.</param>
        /// <returns></returns>
        public async Task UpdateOrderQuantityByProductIdAsync(int productId, int newQuantity)
        {
            if (await _orderValidationService.OrderExistsByProductIdAsync(productId))
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.ProductId == productId);
                if (order != null)
                {
                    order.Quantity = newQuantity;
                    order.CreateOrder = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                throw new CustomException(HttpStatusCode.BadRequest, "The order with the specified product ID does not exist.");
            }
        }

        /// <summary>
        /// Updates the quantity of an order by the product name and size.
        /// </summary>
        /// <param name="productName">The name of the product.</param>
        /// <param name="productSize">The size of the product.</param>
        /// <param name="newQuantity">The new quantity.</param>
        /// <returns></returns>
        public async Task UpdateOrderQuantityByProductNameAndSizeAsync(
            string productName,
            string productSize,
            int newQuantity)
        {
            if (await _orderValidationService.OrderExistsByProductNameAndSizeAsync(productName, productSize))
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o =>
                    o.Product.ProductName == productName &&
                    o.Product.ProductSize == productSize
                );
                if (order != null)
                {
                    order.Quantity = newQuantity;
                    order.CreateOrder = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                throw new CustomException(HttpStatusCode.BadRequest, "The order with the specified product name and size does not exist.");
            }
        }

        /// <summary>
        /// Deletes an order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <returns></returns>
        public async Task DeleteOrderByIdAsync(int id)
        {
            if (await _orderValidationService.OrderExistsByIdAsync(id))
            {
                var order = await _context.Orders.FindAsync(id);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new CustomException(HttpStatusCode.BadRequest, "The order with the specified ID does not exist.");
            }
        }

        /// <summary>
        /// Deletes an order by the product ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns></returns>
        public async Task DeleteOrderByProductIdAsync(int productId)
        {
            if (await _orderValidationService.OrderExistsByProductIdAsync(productId))
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.ProductId == productId);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new CustomException(HttpStatusCode.BadRequest, "The order with the specified product ID does not exist.");
            }
        }

        /// <summary>
        /// Deletes an order by the product name and size.
        /// </summary>
        /// <param name="productName">The name of the product.</param>
        /// <param name="productSize">The size of the product.</param>
        /// <returns></returns>
        public async Task DeleteOrderByProductNameAndSizeAsync(string productName, string productSize)
        {
            if (await _orderValidationService.OrderExistsByProductNameAndSizeAsync(productName, productSize))
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o =>
                    o.Product.ProductName == productName &&
                    o.Product.ProductSize == productSize
                );
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new CustomException(HttpStatusCode.BadRequest, "The order with the specified product name and size does not exist.");
            }
        }

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <returns>A list of orders.</returns>
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            return await _context.Orders.Include(o => o.Product).ToListAsync();
        }

        /// <summary>
        /// Retrieves an order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <returns>The corresponding order.</returns>
        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders.Include(o => o.Product)
                                            .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
                throw new CustomException(HttpStatusCode.BadRequest, "Order empty");
            return order;
        }

        /// <summary>
        /// Retrieves an order by the product name and size.
        /// </summary>
        /// <param name="productName">The name of the product.</param>
        /// <param name="productSize">The size of the product.</param>
        /// <returns>The corresponding order.</returns>
        // public async Task<OrderDto> GetOrderByProductNameAndSizeAsync(string productName, string productSize)
        // {
        //     return await _context.Orders.Include(o => o.Product).FirstOrDefaultAsync(o =>
        //         o.Product.ProductName == productName &&
        //         o.Product.ProductSize == productSize
        //     );
        // }
        public async Task<OrderDto> GetOrderByProductNameAndSizeAsync(string productName, string productSize)
        {
            var order = await _context.Orders.Include(o => o.Product)
                                            .FirstOrDefaultAsync(o =>
                                                o.Product.ProductName == productName &&
                                                o.Product.ProductSize == productSize
                                            );
            if (order == null)
                throw new CustomException(HttpStatusCode.BadRequest, $"Order with product name '{productName}' and size '{productSize}' not found.");
            return order;
        }
    }
}
