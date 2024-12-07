using Market.Market.Models;
using Market.DataModels.EFModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Market.DAL.IDAL;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Market.DAL
{
    public class CartService : ICartService
    {
        private readonly MarketDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartService> _logger;

        public CartService(MarketDbContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<CartService> logger)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<Cart> AddItemToCartAsync(string productName, int quantity, string size)
        {
            _logger.LogDebug("Adding item to cart: Product={ProductName}, Quantity={Quantity}, Size={Size}.", productName, quantity, size);
            try
            {
                var customerId = GetCustomerIdFromContext();
                _logger.LogDebug("Customer ID retrieved: {CustomerId}.", customerId);

                var product = await ValidateAndGetProductAsync(productName, quantity);
                _logger.LogDebug("Product validated and retrieved: {ProductName}.", productName);

                var cart = await GetOrCreateCartAsync(customerId);
                _logger.LogDebug("Cart retrieved or created for Customer ID: {CustomerId}.", customerId);

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == product.Id && i.Size == size);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    _dbContext.CartItem.Update(existingItem);
                    _logger.LogDebug("Updated quantity for existing item in cart: Product={ProductName}, NewQuantity={Quantity}.", productName, existingItem.Quantity);
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
                    _logger.LogDebug("Added new item to cart: Product={ProductName}, Quantity={Quantity}, Size={Size}.", productName, quantity, size);
                }

                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.ProductId == product.Id);
                if (order != null)
                {
                    order.Quantity -= quantity;
                    _dbContext.Orders.Update(order);
                    _logger.LogDebug("Inventory updated for Product={ProductName}, RemainingQuantity={Quantity}.", productName, order.Quantity);
                }

                cart.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();
                _logger.LogDebug("Changes saved to the database.");
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart: Product={ProductName}, Quantity={Quantity}, Size={Size}.", productName, quantity, size);
                throw;
            }
        }

        public async Task<Cart> GetCustomerCartAsync()
        {
            _logger.LogDebug("Retrieving customer cart.");
            var customerId = GetCustomerIdFromContext();
            _logger.LogDebug("Customer ID retrieved: {CustomerId}.", customerId);

            var cart = await _dbContext.Cart
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                _logger.LogError("Cart not found for Customer ID: {CustomerId}.", customerId);
                throw new InvalidOperationException("El cliente no tiene un carrito asociado.");
            }

            _logger.LogDebug("Cart retrieved successfully for Customer ID: {CustomerId}.", customerId);
            return cart;
        }

        public async Task<bool> RemoveItemFromCartAsync(string productName, string size)
        {
            _logger.LogDebug("Removing item from cart: Product={ProductName}, Size={Size}.", productName, size);
            try
            {
                var customerId = GetCustomerIdFromContext();
                _logger.LogDebug("Customer ID retrieved: {CustomerId}.", customerId);

                var cart = await _dbContext.Cart
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (cart == null)
                {
                    _logger.LogError("Cart not found for Customer ID: {CustomerId}.", customerId);
                    throw new InvalidOperationException("El cliente no tiene un carrito.");
                }

                var cartItem = cart.Items.FirstOrDefault(i => i.Product.Name == productName && i.Size == size);
                if (cartItem == null)
                {
                    _logger.LogWarning("Item not found in cart: Product={ProductName}, Size={Size}.", productName, size);
                    return false;
                }

                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.ProductId == cartItem.ProductId);
                if (order != null)
                {
                    order.Quantity += cartItem.Quantity;
                    _dbContext.Orders.Update(order);
                    _logger.LogDebug("Inventory updated for Product={ProductName}, RestoredQuantity={Quantity}.", productName, cartItem.Quantity);
                }

                _dbContext.CartItem.Remove(cartItem);
                await _dbContext.SaveChangesAsync();
                _logger.LogDebug("Item removed from cart successfully: Product={ProductName}, Size={Size}.", productName, size);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart: Product={ProductName}, Size={Size}.", productName, size);
                throw;
            }
        }

        private async Task<Product> ValidateAndGetProductAsync(string productName, int quantity)
        {
            _logger.LogDebug("Validating product: {ProductName}.", productName);
            var productDto = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductName == productName);
            if (productDto == null)
            {
                _logger.LogError("Product not found: {ProductName}.", productName);
                throw new ArgumentException($"El producto '{productName}' no existe.");
            }

            var inventory = await _dbContext.Orders
                .Where(o => o.ProductId == productDto.Id)
                .SumAsync(o => o.Quantity);

            if (inventory < quantity)
            {
                _logger.LogError("Insufficient inventory for Product={ProductName}, RequestedQuantity={Quantity}, AvailableQuantity={Inventory}.", productName, quantity, inventory);
                throw new InvalidOperationException($"Cantidad insuficiente en inventario para el producto '{productName}'.");
            }

            _logger.LogDebug("Product validated successfully: {ProductName}.", productName);
            return new Product
            {
                Id = productDto.Id,
                Name = productDto.ProductName,
                Description = productDto.ProductDescription,
                Price = productDto.ProductPrice,
                Size = productDto.ProductSize
            };
        }

        private int GetCustomerIdFromContext()
        {
            _logger.LogDebug("Retrieving Customer ID from context.");
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                _logger.LogError("Unauthenticated user.");
                throw new UnauthorizedAccessException("Usuario no autenticado.");
            }

            var customerIdClaim = user.FindFirst("customer_id");
            if (customerIdClaim == null)
            {
                _logger.LogError("Customer ID claim not found.");
                throw new UnauthorizedAccessException("El CustomerId no se encuentra en los claims.");
            }

            var customerId = int.Parse(customerIdClaim.Value);
            _logger.LogDebug("Customer ID retrieved: {CustomerId}.", customerId);
            return customerId;
        }

        private async Task<Cart> GetOrCreateCartAsync(int customerId)
        {
            _logger.LogDebug("Retrieving or creating cart for Customer ID: {CustomerId}.", customerId);
            var cart = await _dbContext.Cart
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                _logger.LogDebug("Cart not found. Creating new cart for Customer ID: {CustomerId}.", customerId);
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _dbContext.Cart.AddAsync(cart);
                await _dbContext.SaveChangesAsync();
                _logger.LogDebug("New cart created successfully for Customer ID: {CustomerId}.", customerId);
            }

            return cart;
        }
    }
}
