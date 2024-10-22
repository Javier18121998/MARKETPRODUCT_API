using Market.DAL.IDAL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Market.DataValidation.IDataBaseValidations;
using Market.Market.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.DAL
{
    public class ProductService : IProductService
    {
        private readonly MarketDbContext _context;
        private readonly IProductValidationService _productValidationService;

        public ProductService(MarketDbContext context, IProductValidationService productValidationService)
        {
            _context = context;
            _productValidationService = productValidationService;
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">The product to be created.</param>
        /// <returns>The created product.</returns>
        public async Task<ProductDto> CreateProductAsync(Product product)
        {
            var productCreated = new ProductDto
            {
                Id = product.Id,
                ProductName = product.Name,
                ProductDescription = product.Description,
                ProductPrice = product.Price,
                ProductSize = product.Size,
            };
            _context.Products.Add(productCreated);
            await _context.SaveChangesAsync();
            return productCreated;
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns></returns>
        public async Task DeleteProductByIdAsync(int id)
        {
            if (await _productValidationService.ProductExistsByIdAsync(id))
            {
                var product = await _context.Products.FindAsync(id);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("The product with the specified ID does not exist.");
            }
        }

        /// <summary>
        /// Deletes a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="size">The size of the product.</param>
        /// <returns></returns>
        public async Task DeleteProductByNameAndSizeAsync(string name, string size)
        {
            if (await _productValidationService.ProductExistsByNameAndSizeAsync(name, size))
            {
                var product = await _context.Products.FirstOrDefaultAsync(
                    p => p.ProductName == name &&
                    p.ProductSize == size
                );
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("The product with the specified name and size does not exist.");
            }
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A list of all products.</returns>
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            return products;
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The corresponding product.</returns>
        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var productById = await _context.Products.FindAsync(id);
            return productById;
        }

        /// <summary>
        /// Retrieves a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="size">The size of the product.</param>
        /// <returns>The corresponding product.</returns>
        public async Task<ProductDto> GetProductByNameAndSizeAsync(string name, string size)
        {
            var productByNameAndSize = await _context.Products.FirstOrDefaultAsync(
                p => p.ProductName == name &&
                p.ProductSize == size
            );
            return productByNameAndSize;
        }

        /// <summary>
        /// Updates the description of a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="newDescription">The new description for the product.</param>
        /// <returns></returns>
        public async Task UpdateDescriptionByIdAsync(int id, string newDescription)
        {
            if (await _productValidationService.ProductExistsByIdAsync(id))
            {
                var product = await _context.Products.FindAsync(id);
                product.ProductDescription = newDescription;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("The product with the specified ID does not exist.");
            }
        }

        /// <summary>
        /// Updates the description of a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="size">The size of the product.</param>
        /// <param name="newDescription">The new description for the product.</param>
        /// <returns></returns>
        public async Task UpdateDescriptionByNameAndSizeAsync(string name, string size, string newDescription)
        {
            if (await _productValidationService.ProductExistsByNameAndSizeAsync(name, size))
            {
                var product = await _context.Products.FirstOrDefaultAsync(
                    p => p.ProductName == name &&
                    p.ProductSize == size
                );
                product.ProductDescription = newDescription;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("The product with the specified name and size does not exist.");
            }
        }
    }
}
