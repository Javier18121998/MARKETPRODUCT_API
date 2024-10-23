using Market.DataValidation.IDataBaseValidations;
using Market.Market.Models;
using Microsoft.EntityFrameworkCore;

namespace Market.DataValidation.DataBaseValidations
{
    /// <summary>
    /// The OrderValidationService class provides methods to validate orders in the database.
    /// </summary>
    public class OrderValidationService : IOrderValidationService
    {
        private readonly MarketDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderValidationService"/> class.
        /// </summary>
        /// <param name="context">The database context used for accessing order data.</param>
        public OrderValidationService(MarketDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks if an order exists by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to check.</param>
        /// <returns>A task that represents the asynchronous operation, with a boolean result indicating whether the order exists.</returns>
        public async Task<bool> OrderExistsByIdAsync(int id)
        {
            return await _context.Orders.AnyAsync(o => o.Id == id);
        }

        /// <summary>
        /// Checks if an order exists by its product ID.
        /// </summary>
        /// <param name="productId">The ID of the product to check.</param>
        /// <returns>A task that represents the asynchronous operation, with a boolean result indicating whether the order exists for the specified product ID.</returns>
        public async Task<bool> OrderExistsByProductIdAsync(int productId)
        {
            return await _context.Orders.AnyAsync(o => o.ProductId == productId);
        }

        /// <summary>
        /// Checks if an order exists by product name and size.
        /// </summary>
        /// <param name="productName">The name of the product to check.</param>
        /// <param name="productSize">The size of the product to check.</param>
        /// <returns>A task that represents the asynchronous operation, with a boolean result indicating whether the order exists for the specified product name and size.</returns>
        public async Task<bool> OrderExistsByProductNameAndSizeAsync(string productName, string productSize)
        {
            return await _context.Orders.AnyAsync(o => o.Product.ProductName == productName && o.Product.ProductSize == productSize);
        }
    }
}
