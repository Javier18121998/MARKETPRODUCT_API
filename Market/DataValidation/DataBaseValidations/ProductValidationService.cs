using Market.DataValidation.IDataBaseValidations;
using Market.Market.Models;
using Microsoft.EntityFrameworkCore;

namespace Market.DataValidation.DataBaseValidations
{
    /// <summary>
    /// The ProductValidationService class provides methods to validate products in the database.
    /// </summary>
    public class ProductValidationService : IProductValidationService
    {
        private readonly MarketDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductValidationService"/> class.
        /// </summary>
        /// <param name="context">The database context used for accessing product data.</param>
        public ProductValidationService(MarketDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks if a product exists by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to check.</param>
        /// <returns>A task that represents the asynchronous operation, with a boolean result indicating whether the product exists.</returns>
        public async Task<bool> ProductExistsByIdAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        /// <summary>
        /// Checks if a product exists by its name and size.
        /// </summary>
        /// <param name="name">The name of the product to check.</param>
        /// <param name="size">The size of the product to check.</param>
        /// <returns>A task that represents the asynchronous operation, with a boolean result indicating whether the product exists for the specified name and size.</returns>
        public async Task<bool> ProductExistsByNameAndSizeAsync(string name, string size)
        {
            return await _context.Products.AnyAsync(p => p.ProductName == name && p.ProductSize == size);
        }
    }
}
