using Market.DAL.IDAL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Market.DataValidation.IDataBaseValidations;
using Market.Market.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Market.DAL
{
    public class OrderService : IOrderService
    {
        private readonly MarketDbContext _context;
        private readonly IOrderValidationService _orderValidationService;

        public OrderService(MarketDbContext context, IOrderValidationService orderValidationService)
        {
            _context = context;
            _orderValidationService = orderValidationService;
        }

        // Crear una nueva orden
        public async Task<OrderDto> CreateOrderByProductNameAndSizeAsync(string productName, string productSize, int quantity)
        {
            // Buscar el producto en la base de datos por su nombre y tamaño
            var product = await _context.Products.FirstOrDefaultAsync(
                p => p.ProductName == productName && 
                p.ProductSize == productSize
            );

            if (product == null)
            {
                throw new Exception("El producto con el nombre y tamaño especificado no existe.");
            }

            // Crear la orden con el ID del producto y la cantidad
            var order = new OrderDto
            {
                ProductId = product.Id,
                Quantity = quantity,
                CreateOrder = DateTime.UtcNow
            };

            // Agregar la orden a la base de datos
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        // Crear una nueva orden por ID del producto
        public async Task<OrderDto> CreateOrderByProductIdAsync(int productId, int quantity)
        {
            // Validar si el producto existe por su ID
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new Exception("El producto con el ID especificado no existe.");
            }

            // Crear la orden con el ID del producto y la cantidad
            var order = new OrderDto
            {
                ProductId = productId,
                Quantity = quantity,
                CreateOrder = DateTime.UtcNow
            };

            // Agregar la orden a la base de datos
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        // Actualizar cantidad de una orden por ID de la orden
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
                throw new Exception("La orden con el ID especificado no existe.");
            }
        }

        // Actualizar cantidad de una orden por ID del producto
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
                throw new Exception("La orden con el ID del producto especificado no existe.");
            }
        }

        // Actualizar cantidad de una orden por nombre y tamaño del producto
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
                throw new Exception("La orden con el nombre y tamaño del producto especificado no existe.");
            }
        }

        // Eliminar una orden por ID
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
                throw new Exception("La orden con el ID especificado no existe.");
            }
        }

        // Eliminar una orden por ID del producto
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
                throw new Exception("La orden con el ID del producto especificado no existe.");
            }
        }

        // Eliminar una orden por nombre y tamaño del producto
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
                throw new Exception("La orden con el nombre y tamaño del producto especificado no existe.");
            }
        }

        // Obtener todas las órdenes
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            return await _context.Orders.Include(o => o.Product).ToListAsync();
        }

        // Obtener una orden por ID
        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.Product)
                                        .FirstOrDefaultAsync(o => o.Id == id);
        }

        // Obtener una orden por nombre y tamaño del producto
        public async Task<OrderDto> GetOrderByProductNameAndSizeAsync(string productName, string productSize)
        {
            return await _context.Orders.Include(o => o.Product).FirstOrDefaultAsync(o => 
                o.Product.ProductName == productName && 
                o.Product.ProductSize == productSize
            );
        }
    }
}
