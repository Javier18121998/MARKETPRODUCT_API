using Market.BL.IBL;
using Market.DAL.IDAL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.BL
{
    public class ProductServiceBL : IProductServiceBL
    {
        private readonly IProductService _productService;

        public ProductServiceBL(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">The product to create.</param>
        /// <returns>The created product DTO.</returns>
        public async Task<ProductDto> CreateProductAsync(Product product)
        {
            try
            {
                var productCreated = await _productService.CreateProductAsync(product);
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                return productCreated;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        public async Task DeleteProductByIdAsync(int id)
        {
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                await _productService.DeleteProductByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product to delete.</param>
        /// <param name="size">The size of the product to delete.</param>
        public async Task DeleteProductByNameAndSizeAsync(string name, string size)
        {
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                await _productService.DeleteProductByNameAndSizeAsync(name, size);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A collection of product DTOs.</returns>
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                return products;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The corresponding product DTO.</returns>
        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product to retrieve.</param>
        /// <param name="size">The size of the product to retrieve.</param>
        /// <returns>The corresponding product DTO.</returns>
        public async Task<ProductDto> GetProductByNameAndSizeAsync(string name, string size)
        {
            try
            {
                var productByNameAndSize = await _productService.GetProductByNameAndSizeAsync(name, size);
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                return productByNameAndSize;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates the description of a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="newDescription">The new description for the product.</param>
        public async Task UpdateDescriptionByIdAsync(int id, string newDescription)
        {
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                await _productService.UpdateDescriptionByIdAsync(id, newDescription);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates the description of a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product to update.</param>
        /// <param name="size">The size of the product to update.</param>
        /// <param name="newDescription">The new description for the product.</param>
        public async Task UpdateDescriptionByNameAndSizeAsync(string name, string size, string newDescription)
        {
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                #endregion
                await _productService.UpdateDescriptionByNameAndSizeAsync(name, size, newDescription);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
