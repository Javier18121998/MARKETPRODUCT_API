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

        private int GetCustomerIdFromContext()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("No se pudo obtener el CustomerId.");
            }

            var customerIdClaim = user.FindFirst("customer_id");
            if (customerIdClaim == null)
            {
                throw new UnauthorizedAccessException("El CustomerId no se encuentra en los claims.");
            }

            return int.Parse(customerIdClaim.Value);
        }


        public async Task<Cart> AddItemToCartAsync(string productName, int quantity, string size)
        {
            try
            {
                var customerId = GetCustomerIdFromContext();

                // Buscar el producto por nombre
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductName == productName);
                if (product == null)
                {
                    throw new ArgumentException("Producto no encontrado.");
                }

                // Verificar existencia en Orders para actualizar inventario
                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.ProductId == product.Id);
                if (order == null || order.Quantity < quantity)
                {
                    throw new InvalidOperationException("Cantidad insuficiente en inventario.");
                }

                // Obtener o crear el carrito del cliente
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
                    _dbContext.Cart.Add(cart);
                    await _dbContext.SaveChangesAsync();
                }

                // Revisar si ya existe el producto en el carrito
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
                        Size = size,
                    };
                    cart.Items.Add(cartItem);
                    _dbContext.CartItem.Add(cartItem);
                }

                // Actualizar inventario en Orders
                order.Quantity -= quantity;
                _dbContext.Orders.Update(order);

                // Actualizar la fecha de modificación del carrito
                cart.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return cart;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message, "Argumento inválido en AddItemToCartAsync: {Message}", ex.Message);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message, "Operación inválida en AddItemToCartAsync: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error inesperado en AddItemToCartAsync: {Message}", ex.Message);
                throw new Exception("Error al añadir el producto al carrito, intente de nuevo.");
            }
        }


        public async Task<Cart> GetCustomerCartAsync()
        {
            var customerId = GetCustomerIdFromContext();

            return await _dbContext.Cart
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<bool> RemoveItemFromCartAsync(string productName, string size)
        {
            var customerId = GetCustomerIdFromContext(); // Usar el método que ya tienes para obtener el ID del cliente

            // Obtener el carrito del cliente autenticado
            var cart = await _dbContext.Cart
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                return false; // No se encontró el carrito para el cliente
            }

            // Buscar el item en el carrito por nombre del producto y tamaño
            var cartItem = cart.Items.FirstOrDefault(i => i.Product.Name == productName && i.Size == size);
            if (cartItem == null)
            {
                return false; // No se encontró el producto en el carrito
            }

            // Actualizar inventario de Orders al remover el item del carrito
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.ProductId == cartItem.ProductId);
            if (order != null)
            {
                order.Quantity += cartItem.Quantity;
                _dbContext.Orders.Update(order);
            }

            // Eliminar el CartItem del carrito y guardar cambios
            _dbContext.CartItem.Remove(cartItem);
            await _dbContext.SaveChangesAsync();
            return true;
        }


    }
}
