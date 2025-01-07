using Market.Market.Models;
using Market.DataModels.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Market.DAL.IDAL;
using Market.DataModels.DTos;

namespace Market.DAL
{
    /// <summary>
    /// Service class that handles cart-related operations such as adding/removing items, retrieving the cart, and calculating total payment.
    /// </summary>
    public class CartService : ICartService
    {
        private readonly MarketDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context used for accessing the data.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor for retrieving customer data.</param>
        /// <param name="logger">The logger for logging cart operations.</param>
        public CartService(
            MarketDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CartService> logger)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Adds a product to the customer's cart. If the product is already in the cart, it updates the quantity.
        /// </summary>
        /// <param name="productName">The name of the product to add.</param>
        /// <param name="quantity">The quantity of the product to add.</param>
        /// <param name="size">The size of the product.</param>
        /// <returns>The updated cart.</returns>
        /// <exception cref="ArgumentException">Thrown when the product doesn't exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown when there is insufficient inventory.</exception>
        public async Task<Cart> AddItemToCartAsync(string productName, int quantity, string size)
        {
            try
            {
                _logger.LogDebug("Adding item to cart: Product={ProductName}, Quantity={Quantity}, Size={Size}.", productName, quantity, size);

                var customerId = GetCustomerIdFromContext();
                var product = await ValidateAndGetProductAsync(productName, quantity);
                var cart = await GetOrCreateCartAsync(customerId);

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
                    await _dbContext.CartItem.AddAsync(cartItem);
                    cart.Items.Add(cartItem);
                }

                await UpdateInventoryAsync(product.Id, -quantity);

                cart.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                _logger.LogDebug("Item added to cart successfully: Product={ProductName}, Quantity={Quantity}, Size={Size}.", productName, quantity, size);
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the customer's cart, including the items and their associated products.
        /// </summary>
        /// <returns>The customer's cart.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the cart is not found for the customer.</exception>
        public async Task<Cart> GetCustomerCartAsync()
        {
            try
            {
                var customerId = GetCustomerIdFromContext();
                var cart = await _dbContext.Cart
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (cart == null)
                {
                    _logger.LogWarning("Cart not found for Customer ID: {CustomerId}.", customerId);
                    throw new InvalidOperationException("El cliente no tiene un carrito asociado.");
                }

                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer cart.");
                throw;
            }
        }

        /// <summary>
        /// Removes an item from the customer's cart.
        /// </summary>
        /// <param name="productName">The name of the product to remove.</param>
        /// <param name="size">The size of the product to remove.</param>
        /// <returns>True if the item was removed successfully, otherwise false.</returns>
        public async Task<bool> RemoveItemFromCartAsync(string productName, string size)
        {
            try
            {
                var customerId = GetCustomerIdFromContext();
                var cart = await _dbContext.Cart
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (cart == null)
                {
                    _logger.LogWarning("Cart not found for Customer ID: {CustomerId}.", customerId);
                    return false;
                }

                var cartItem = cart.Items.FirstOrDefault(i => i.Product.Name == productName && i.Size == size);
                if (cartItem == null)
                {
                    _logger.LogWarning("Item not found in cart: Product={ProductName}, Size={Size}.", productName, size);
                    return false;
                }

                await UpdateInventoryAsync(cartItem.ProductId, cartItem.Quantity);
                _dbContext.CartItem.Remove(cartItem);
                await _dbContext.SaveChangesAsync();

                _logger.LogDebug("Item removed from cart successfully: Product={ProductName}, Size={Size}.", productName, size);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart.");
                throw;
            }
        }

        /// <summary>
        /// Calculates the total payment for the items in the customer's cart.
        /// </summary>
        /// <returns>The total payment amount.</returns>
        public async Task<decimal> TotalPaymentOfCartAsync()
        {
            try
            {
                var customerId = GetCustomerIdFromContext();
                var cart = await _dbContext.Cart
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (cart == null || cart.Items == null || !cart.Items.Any())
                {
                    _logger.LogInformation("Cart is empty or not found for Customer ID: {CustomerId}.", customerId);
                    return 0m;
                }

                var totalPayment = cart.Items.Sum(item => item.Product.Price * item.Quantity);
                return totalPayment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total payment for cart.");
                throw;
            }
        }

        #region CartService Functionalities and Validations
        /// <summary>
        /// Validates the product and checks if the requested quantity is available in inventory.
        /// </summary>
        /// <param name="productName">The name of the product to validate.</param>
        /// <param name="quantity">The quantity of the product requested.</param>
        /// <returns>A <see cref="ProductDto"/> containing the product's details.</returns>
        /// <exception cref="ArgumentException">Thrown when the product does not exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown when there is insufficient inventory.</exception>
        private async Task<ProductDto> ValidateAndGetProductAsync(string productName, int quantity)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductName == productName);
            if (product == null)
                throw new ArgumentException($"El producto '{productName}' no existe.");

            var inventory = await _dbContext.Orders
                .Where(o => o.ProductId == product.Id)
                .SumAsync(o => o.Quantity);

            if (inventory < quantity)
                throw new InvalidOperationException($"Cantidad insuficiente en inventario para el producto '{productName}'.");

            return product;
        }

        /// <summary>
        /// Updates the inventory based on the product's ID and the quantity change (positive for adding, negative for removing).
        /// </summary>
        /// <param name="productId">The product's ID to update the inventory for.</param>
        /// <param name="quantityDelta">The change in inventory quantity (positive or negative).</param>
        private async Task UpdateInventoryAsync(int productId, int quantityDelta)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.ProductId == productId);
            if (order == null)
                throw new InvalidOperationException($"No se encontró inventario para el producto ID: {productId}.");

            order.Quantity += quantityDelta;
            if (order.Quantity < 0)
                throw new InvalidOperationException($"Cantidad insuficiente en inventario para el producto ID: {productId}.");

            _dbContext.Orders.Update(order);
        }

        /// <summary>
        /// Retrieves the customer ID from the HTTP context based on the user's authentication claims.
        /// </summary>
        /// <returns>The customer ID.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if the user is not authenticated or the customer ID claim is not found.</exception>
        private int GetCustomerIdFromContext()
        {
            _logger.LogDebug("Retrieving Customer ID from context.");
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
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


        /// <summary>
        /// Retrieves the customer's cart, or creates a new one if it does not exist.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>The customer's cart.</returns>
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
        #endregion
    }
}
