namespace Market.DataValidation.IDataBaseValidations
{
    /// <summary>
    /// The IProductValidationService interface defines the contract for product validation services.
    /// </summary>
    public interface IProductValidationService
    {
        /// <summary>
        /// Asynchronously checks if a product exists by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to check.</param>
        /// <returns>A task that represents the asynchronous operation, with a boolean result indicating whether the product exists.</returns>
        Task<bool> ProductExistsByIdAsync(int id);

        /// <summary>
        /// Asynchronously checks if a product exists by its name and size.
        /// </summary>
        /// <param name="name">The name of the product to check.</param>
        /// <param name="size">The size of the product to check.</param>
        /// <returns>A task that represents the asynchronous operation, with a boolean result indicating whether the product exists for the specified name and size.</returns>
        Task<bool> ProductExistsByNameAndSizeAsync(string name, string size);
    }
}
