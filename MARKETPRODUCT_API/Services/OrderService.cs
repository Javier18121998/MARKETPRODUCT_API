using MARKETPRODUCT_API.Data.DTOs;
using MARKETPRODUCT_API.Data.EFModels;
using MARKETPRODUCT_API.Data;
using Microsoft.EntityFrameworkCore;
using MARKETPRODUCT_API.Services.IServices;
using MARKETPRODUCT_API.Messaging.MessageProducer;
using MARKETPRODUCT_API.Messaging.MessageModels;

namespace MARKETPRODUCT_API.Services
{
    public class OrderService: IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly OrderMQProducer _orderMQProducer;

        public OrderService(ApplicationDbContext context, OrderMQProducer orderMQProducer)
        {
            _context = context;
            _orderMQProducer = orderMQProducer;
        }

        public OrderDto CreateOrder(CreateOrderDto createOrderDto)
        {
            var order = new Order();

            foreach (var itemDto in createOrderDto.Items)
            {
                var product = _context.Products.Find(itemDto.ProductId);
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

            _context.Orders.Add(order);
            _context.SaveChanges();

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
            _orderMQProducer.SendMessage(new OrderServiceMessage
            {
                Id = order.Id,
                CreationDate = DateTime.UtcNow,
                ProductId = orderCreated.Items.First().ProductId.ToString(),
                ProductName = orderCreated.Items.First().ProductName,
                Quantity = orderCreated.Items.First().Quantity.ToString()
            });
            return orderCreated;
        }

        public OrderDto GetOrderById(int id)
        {
            var order = _context.Orders.Include(o => o.Items).ThenInclude(oi => oi.Product).FirstOrDefault(o => o.Id == id);
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

        public IEnumerable<OrderDto> GetAllOrders()
        {
            return _context.Orders
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
                }).ToList();
        }

        public void DeleteOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                throw new Exception($"Order with ID {id} not found");
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();
        }
    }
}
