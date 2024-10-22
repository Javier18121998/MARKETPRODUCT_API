using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Market.DataModels.EFModels;
using System.Threading.Tasks;
using Market.DataModels.DTos;

namespace Market.BL.IBL
{
    public interface IProductServiceBL
    {
        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A collection of product data transfer objects (DTOs).</returns>
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The corresponding product DTO.</returns>
        Task<ProductDto> GetProductByIdAsync(int id);

        /// <summary>
        /// Retrieves a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="size">The size of the product.</param>
        /// <returns>The corresponding product DTO.</returns>
        Task<ProductDto> GetProductByNameAndSizeAsync(string name, string size);

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">The product entity to create.</param>
        /// <returns>The created product DTO.</returns>
        Task<ProductDto> CreateProductAsync(Product product);

        /// <summary>
        /// Updates the description of a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="newDescription">The new description for the product.</param>
        /// <returns></returns>
        Task UpdateDescriptionByIdAsync(int id, string newDescription);

        /// <summary>
        /// Updates the description of a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="size">The size of the product.</param>
        /// <param name="newDescription">The new description for the product.</param>
        /// <returns></returns>
        Task UpdateDescriptionByNameAndSizeAsync(string name, string size, string newDescription);

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns></returns>
        Task DeleteProductByIdAsync(int id);

        /// <summary>
        /// Deletes a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="size">The size of the product.</param>
        /// <returns></returns>
        Task DeleteProductByNameAndSizeAsync(string name, string size);
    }
}
