namespace Market.DataValidation.IDataBaseValidations
{
    /// <summary>
    /// The IOrderValidationService interface defines the contract for order validation services.
    /// </summary>
    public interface IOrderValidationService
    {
        /// <summary>
        /// Asynchronously checks if an order exists by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to check.</param>
        /// <returns>A task that represents the asynchronous operation, with a boolean result indicating whether the order exists.</returns>
        Task<bool> OrderExistsByIdAsync(int id);

        /// <summary>
        /// Asynchronously checks if an order exists by its product ID.
        /// </summary>
        /// <param name="productId">The ID of the product to check.</param>
        /// <returns>A task that represents the asynchronous operation, with a boolean result indicating whether the order exists for the specified product ID.</returns>
        Task<bool> OrderExistsByProductIdAsync(int productId);

        /// <summary>
        /// Asynchronously checks if an order exists by product name and size.
        /// </summary>
        /// <param name="productName">The name of the product to check.</param>
        /// <param name="productSize">The size of the product to check.</param>
        /// <returns>A task that represents the asynchronous operation, with a boolean result indicating whether the order exists for the specified product name and size.</returns>
        Task<bool> OrderExistsByProductNameAndSizeAsync(string productName, string productSize);
    }
}
