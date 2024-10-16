using MARKETPRODUCT_API.Data.DTOs;
using MARKETPRODUCT_API.Data.EFModels;
using MARKETPRODUCT_API.Data;
using Microsoft.EntityFrameworkCore;
using MARKETPRODUCT_API.Services.IServices;
using MARKETPRODUCT_API.Messaging.MessageProducer;
using MARKETPRODUCT_API.Messaging.MessageModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace MARKETPRODUCT_API.Services
{
    /// <summary>
    /// This service cordinates the operations into an Order
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly MQProducer _MQProducer;

        public OrderService(ApplicationDbContext context, MQProducer MQProducer)
        {
            _context = context;
            _MQProducer = MQProducer;
        }

        /// <summary>
        /// This operation creates the order per product Id and quantity by a CreateOrderDto with many Items
        /// </summary>
        /// <param name="createOrderDto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto) 
        {
            var order = new Order();

            foreach (var itemDto in createOrderDto.Items)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product with ID {itemDto.ProductId} not found");
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    Product = product
                };
                order.Items.Add(orderItem);
            }

            await _context.Orders.AddAsync(order); 
            await _context.SaveChangesAsync(); 

            var orderCreated = new OrderDto
            {
                Id = order.Id,
                CreatedDate = order.CreatedDate,
                Items = order.Items.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductPrice = oi.Product.Price,
                    Quantity = oi.Quantity
                }).ToList()
            };

            _MQProducer.SendMessage(new OrderServiceMessage
            {
                Id = order.Id,
                CreationDate = DateTime.UtcNow,
                ProductId = orderCreated.Items.First().ProductId.ToString(),
                ProductName = orderCreated.Items.First().ProductName,
                Quantity = orderCreated.Items.First().Quantity.ToString()
            });

            return orderCreated;
        }

        /// <summary>
        /// This operation will get all the Orders by their Id in case 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<OrderDto> GetOrderByIdAsync(int id) 
        {
            var order = await _context.Orders.Include(o => o.Items).ThenInclude(oi => oi.Product)
                                               .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                throw new Exception($"Order with ID {id} not found");
            }

            return new OrderDto
            {
                Id = order.Id,
                CreatedDate = order.CreatedDate,
                Items = order.Items.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductPrice = oi.Product.Price,
                    Quantity = oi.Quantity
                }).ToList()
            };
        }

        /// <summary>
        /// This operation manage the fetch of all Orders
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Select(order => new OrderDto
                {
                    Id = order.Id,
                    CreatedDate = order.CreatedDate,
                    Items = order.Items.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        ProductPrice = oi.Product.Price,
                        Quantity = oi.Quantity
                    }).ToList()
                }).ToListAsync();
        }

        /// <summary>
        /// This operation delete in cost an Order by their Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteOrderAsync(int id) 
        {
            var order = await _context.Orders.FindAsync(id); 
            if (order == null)
            {
                throw new Exception($"Order with ID {id} not found");
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync(); 
        }
    }
}
