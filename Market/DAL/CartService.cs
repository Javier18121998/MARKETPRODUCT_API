using Market.Market.Models;
using Market.DataModels.EFModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Market.DAL.IDAL;
using Microsoft.AspNetCore.Http;

namespace Market.DAL
{
    public class CartService : ICartService
    {
        private readonly MarketDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(MarketDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }



        public async Task<Cart> AddItemToCartAsync(string productName, int quantity, string size)
        {
            try
            {
                var customerId = GetCustomerIdFromContext();

                // Validar producto y obtenerlo
                var product = await ValidateAndGetProductAsync(productName, quantity);

                // Obtener o crear el carrito
                var cart = await GetOrCreateCartAsync(customerId);

                // Revisar si el producto ya está en el carrito
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == product.Id && i.Size == size);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    _dbContext.CartItem.Update(existingItem);
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = product.Id,
                        Quantity = quantity,
                        Size = size
                    };
                    cart.Items.Add(cartItem);
                    await _dbContext.CartItem.AddAsync(cartItem);
                }

                // Actualizar inventario
                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.ProductId == product.Id);
                if (order != null)
                {
                    order.Quantity -= quantity;
                    _dbContext.Orders.Update(order);
                }

                // Actualizar la fecha del carrito
                cart.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();
                return cart;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddItemToCartAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Cart> GetCustomerCartAsync()
        {
            var customerId = GetCustomerIdFromContext();

            var cart = await _dbContext.Cart
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
                throw new InvalidOperationException("El cliente no tiene un carrito asociado.");

            return cart;
        }

        public async Task<bool> RemoveItemFromCartAsync(string productName, string size)
        {
            try
            {
                var customerId = GetCustomerIdFromContext();

                // Obtener el carrito del cliente
                var cart = await _dbContext.Cart
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (cart == null)
                    throw new InvalidOperationException("El cliente no tiene un carrito.");

                // Buscar el item por nombre y tamaño
                var cartItem = cart.Items.FirstOrDefault(i => i.Product.Name == productName && i.Size == size);
                if (cartItem == null)
                    return false;

                // Actualizar inventario
                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.ProductId == cartItem.ProductId);
                if (order != null)
                {
                    order.Quantity += cartItem.Quantity;
                    _dbContext.Orders.Update(order);
                }

                // Eliminar el item del carrito
                _dbContext.CartItem.Remove(cartItem);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en RemoveItemFromCartAsync: {ex.Message}");
                throw;
            }
        }

        private async Task<Product> ValidateAndGetProductAsync(string productName, int quantity)
        {
            // Buscar el producto por nombre
            var productDto = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductName == productName);
            if (productDto == null)
                throw new ArgumentException($"El producto '{productName}' no existe.");

            // Verificar el inventario en la tabla de Orders
            var inventory = await _dbContext.Orders
                .Where(o => o.ProductId == productDto.Id)
                .SumAsync(o => o.Quantity);

            if (inventory < quantity)
                throw new InvalidOperationException($"Cantidad insuficiente en inventario para el producto '{productName}'.");

            var product = new Product
            {
                Id = productDto.Id,
                Name = productDto.ProductName,
                Description = productDto.ProductDescription,
                Price = productDto.ProductPrice,
                Size = productDto.ProductSize
            };

            return product;
        }

#pragma warning disable CS8602 
        private int GetCustomerIdFromContext()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("Usuario no autenticado.");

            var customerIdClaim = user.FindFirst("customer_id");
            if (customerIdClaim == null)
                throw new UnauthorizedAccessException("El CustomerId no se encuentra en los claims.");

            return int.Parse(customerIdClaim.Value);
        }
#pragma warning restore CS8602 

        private async Task<Cart> GetOrCreateCartAsync(int customerId)
        {
            var cart = await _dbContext.Cart
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _dbContext.Cart.AddAsync(cart);
                await _dbContext.SaveChangesAsync();
            }

            return cart;
        }
    }
}