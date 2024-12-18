﻿using Market.DAL.IDAL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Market.DataValidation.IDataBaseValidations;
using Market.Exceptions;
using Market.Market.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tensorflow.Contexts;

namespace Market.DAL
{
    /// <summary>
    /// Service for managing products in the market, providing methods for 
    /// creating, deleting, updating, and retrieving product information.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly MarketDbContext _context;
        private readonly IProductValidationService _productValidationService;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
                MarketDbContext context, 
                IProductValidationService productValidationService, 
                ILogger<ProductService> logger
            )
        {
            _context = context;
            _productValidationService = productValidationService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">The product to be created.</param>
        /// <returns>The created product.</returns>
        public async Task<ProductDto> CreateProductAsync(Product product)
        {
            try
            {
                _logger.LogDebug("Starting CreateProductAsync for product: {ProductName}, Size: {Size}", product.Name, product.Size);
                if (IsLiquid(product.Size))
                {
                    await IsLiquidProductDuplicateAsync(product);
                }
                else if (IsSolid(product.Size))
                {
                    await IsSolidProductDuplicateAsync(product);
                }
                else
                {
                    DuplicateProductCheck(product);
                }

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

                _logger.LogDebug("Product created successfully with ID: {ProductId}", productCreated.Id);
                return productCreated;
            }
            catch (CustomException ex)
            {
                _logger.LogError(ex, "Error occurred while creating product: {ProductName}, Size: {Size}", product.Name, product.Size);
                throw new CustomException(ex.StatusCode, ex.Message, ex.ErrorCode);
            }
        }


        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns></returns>
        public async Task DeleteProductByIdAsync(int id)
        {
            _logger.LogDebug("Attempting to delete product with ID: {Id}", id);
            if (await _productValidationService.ProductExistsByIdAsync(id))
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    _logger.LogDebug("Product with ID: {Id} deleted successfully.", id);
                }
                else
                {
                    _logger.LogWarning("Product with ID: {Id} was not found in the database.", id);
                }
            }
            else
            {
                _logger.LogError("Validation failed: Product with ID: {Id} does not exist.", id);
                throw new CustomException(HttpStatusCode.BadRequest, "The product with the specified ID does not exist.", "0004");
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
            _logger.LogDebug("Attempting to delete product with Name: {Name}, Size: {Size}", name, size);
            if (await _productValidationService.ProductExistsByNameAndSizeAsync(name, size))
            {
                var product = await _context.Products.FirstOrDefaultAsync(
                    p => p.ProductName == name &&
                    p.ProductSize == size
                );
                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync(); _logger.LogInformation("Product with Name: {Name}, Size: {Size} deleted successfully.", name, size);
                }
                else
                {
                    _logger.LogWarning("Product with Name: {Name}, Size: {Size} was not found in the database.", name, size);
                }
            }
            else
            {
                _logger.LogError("Validation failed: Product with Name: {Name}, Size: {Size} does not exist.", name, size);
                throw new CustomException(HttpStatusCode.BadRequest, "The product with the specified name and size does not exist.", "0004");
            }
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            _logger.LogDebug("Retrieving all products.");
            var products = await _context.Products.ToListAsync();
            _logger.LogDebug("{Count} products retrieved successfully.", products.Count);
            return products;
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            _logger.LogDebug("Retrieving product with ID: {Id}", id);
            var productById = await _context.Products.FindAsync(id);
            if (productById != null)
            {
                _logger.LogDebug("Product with ID: {Id} retrieved successfully.", id);
                return productById;
            }
            else
            {
                _logger.LogError("Product with ID: {Id} not found.", id);
                throw new CustomException(HttpStatusCode.NotFound, "The product with the specified ID does not exist.", "");
            }
        }

        /// <summary>
        /// Retrieves a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="size">The size of the product.</param>
        /// <returns>The corresponding product.</returns>
        public async Task<ProductDto> GetProductByNameAndSizeAsync(string name, string size)
        {
            _logger.LogDebug("Attempting to retrieve product with Name: {Name} and Size: {Size}.", name, size);
            try
            {
                var productByNameAndSize = await _context.Products.FirstOrDefaultAsync(
                    p => p.ProductName == name &&
                    p.ProductSize == size
                );

                if (productByNameAndSize != null)
                {
                    _logger.LogDebug("Product found with Name: {Name} and Size: {Size}.", name, size);
                    return productByNameAndSize;
                }
                else
                {
                    _logger.LogWarning("No product found with Name: {Name} and Size: {Size}.", name, size);
                    throw new CustomException(HttpStatusCode.NotFound, "The product with the specified Name and Size does not exist. Try with another name or size.", "");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the product with Name: {Name} and Size: {Size}.", name, size);
                throw;
            }
        }


        /// <summary>
        /// Updates the description of a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="newDescription">The new description for the product.</param>
        /// <returns></returns>
        public async Task UpdateDescriptionByIdAsync(int id, string newDescription)
        {
            _logger.LogDebug("Attempting to update description for product with ID: {Id}.", id);
            try
            {
                if (await _productValidationService.ProductExistsByIdAsync(id))
                {
                    var product = await _context.Products.FindAsync(id);
                    if (product != null)
                    {
                        _logger.LogDebug("Product found with ID: {Id}. Updating description.", id);
                        product.ProductDescription = newDescription;
                        await _context.SaveChangesAsync();
                        _logger.LogDebug("Successfully updated description for product with ID: {Id}.", id);
                    }
                }
                else
                {
                    _logger.LogWarning("Product with ID: {Id} does not exist.", id);
                    throw new CustomException(HttpStatusCode.BadRequest, "The product with the specified ID does not exist.", "");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the description for product with ID: {Id}.", id);
                throw;
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
            _logger.LogDebug("Attempting to update description for product with Name: {Name} and Size: {Size}.", name, size);
            try
            {
                if (await _productValidationService.ProductExistsByNameAndSizeAsync(name, size))
                {
                    var product = await _context.Products.FirstOrDefaultAsync(
                        p => p.ProductName == name &&
                        p.ProductSize == size
                    );
                    if (product != null)
                    {
                        _logger.LogDebug("Product found with Name: {Name} and Size: {Size}. Updating description.", name, size);
                        product.ProductDescription = newDescription;
                        await _context.SaveChangesAsync();
                        _logger.LogDebug("Successfully updated description for product with Name: {Name} and Size: {Size}.", name, size);
                    }
                }
                else
                {
                    _logger.LogWarning("Product with Name: {Name} and Size: {Size} does not exist.", name, size);
                    throw new CustomException(HttpStatusCode.BadRequest, "The product with the specified name and size does not exist.", "0004");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the description for product with Name: {Name} and Size: {Size}.", name, size);
                throw;
            }
        }

        /// <summary>
        /// Contains methods to validate if a product is a duplicate based on its name and size.
        /// The logic involves normalizing both the product name and size to ensure consistent comparisons.
        /// If a duplicate product is found, an exception is thrown to indicate the conflict
        /// JFYI The critical validation looks for type of product: solid | liquid
        /// </summary>
        #region DuplicateProducts Registration Logic

        /// <summary>
        /// Validates and checks if a liquid product is a duplicate based on its normalized name and size.
        /// Throws a CustomException if a duplicate is found.
        /// </summary>
        private async Task IsLiquidProductDuplicateAsync(Product product)
        {
            string normalizedName = NormalizeProductName(product.Name);
            string normalizedSize = NormalizeLiquidProductSize(product.Size);
            var existingProducts = await _context.Products.ToListAsync();
            var matchingProducts = existingProducts
                .Where(p => NormalizeProductName(p.ProductName) == normalizedName)
                .ToList();
            if (matchingProducts.Any(existingProduct => NormalizeLiquidProductSize(existingProduct.ProductSize) == normalizedSize))
            {
                throw new CustomException(HttpStatusCode.BadRequest, "The product is already exists.", "404 Not Created");
            }
        }

        /// <summary>
        /// Validates and checks if a solid product is a duplicate based on its normalized name and size.
        /// Throws a CustomException if a duplicate is found.
        /// </summary>
        private async Task IsSolidProductDuplicateAsync(Product product)
        {
            string normalizedName = NormalizeProductName(product.Name);
            string normalizedSize = NormalizeSolidProductSize(product.Size);
            var existingProducts = await _context.Products.ToListAsync();
            var matchingProducts = existingProducts
                .Where(p => NormalizeProductName(p.ProductName) == normalizedName)
                .ToList();
            if (matchingProducts.Any(existingProduct => NormalizeSolidProductSize(existingProduct.ProductSize) == normalizedSize))
            {
                throw new CustomException(HttpStatusCode.BadRequest, "The product is already exists.", "404 Not Created");
            }
        }

        private string NormalizeSolidProductSize(string size)
        {
            string normalizedSize = size.ToLowerInvariant().Trim();
            var sizeEquivalences = new Dictionary<string, string>
            {
                { "1kg", "1000" }, { "1 kg", "1000" }, { "1000g", "1000" },
                { "1000 g", "1000" }, { "1.0kg", "1000" }, { "mil gramos", "1000" },
                { "mil", "1000" }, { "1 kilogramo", "1000" }, { "1.0 kg", "1000" },
                { "1kilo", "1000" }, { "un kilo", "1000" }, { "mil g", "1000" },
                { "milgramos", "1000" },
                { "500g", "500" }, { "500 g", "500" }, { "0.5kg", "500" },
                { "medio kilo", "500" }, { "medio kilogramo", "500" }, { "1/2kg", "500" },
                { "500 gramos", "500" }, { "0.5 kg", "500" }, { "medio", "500" },
                { "500miligramos", "500" }, { "0.5 kilo", "500" }, { "medio kg", "500" },
                { "1/2 kilo", "500" },
                { "250g", "250" }, { "250 g", "250" }, { "0.25kg", "250" },
                { "250 gramos", "250" }, { "1/4kg", "250" }, { "0.25 kilo", "250" },
                { "0.25 kg", "250" }, { "un cuarto de kilo", "250" },
                { "750g", "750" }, { "750 g", "750" }, { "0.75kg", "750" },
                { "750 gramos", "750" }, { "3/4kg", "750" }, { "0.75 kilo", "750" },
                { "0.75 kg", "750" }, { "tres cuartos de kilo", "750" },
                { "100g", "100" }, { "100 g", "100" }, { "0.1kg", "100" },
                { "100 gramos", "100" }, { "0.1 kg", "100" },
                { "50g", "50" }, { "50 g", "50" }, { "0.05kg", "50" },
                { "50 gramos", "50" }, { "0.05 kg", "50" },
                { "1.5kg", "1500" }, { "1.5 kg", "1500" }, { "1500g", "1500" },
                { "1 kilo y medio", "1500" }, { "mil quinientos gramos", "1500" }
            };

            foreach (var equivalence in sizeEquivalences)
            {
                if (normalizedSize.Contains(equivalence.Key))
                {
                    return equivalence.Value;
                }
            }
            if (decimal.TryParse(Regex.Replace(normalizedSize, "[^0-9]", ""), out decimal sizeInGrams))
            {
                return sizeInGrams.ToString();
            }
            return normalizedSize;
        }

        private string NormalizeLiquidProductSize(string size)
        {
            string normalizedSize = size.ToLowerInvariant().Trim();
            var sizeEquivalences = new Dictionary<string, string>
            {
                { "1lt", "1000" }, { "1 lt", "1000" }, { "1 litro", "1000" },
                { "1.0lt", "1000" }, { "1 l", "1000" }, { "1l", "1000" },
                { "1000ml", "1000" }, { "1000 ml", "1000" }, { "mililitros", "1000" },
                { "mil ml", "1000" }, { "mil", "1000" }, { "1.0 l", "1000" },
                { "mililitro", "1000" }, { "1000", "1000" },
                { "500ml", "500" }, { "500 ml", "500" }, { "0.5lt", "500" },
                { "medio litro", "500" }, { "1/2lt", "500" }, { "0.5l", "500" },
                { "medio", "500" }, { "1/2 l", "500" }, { "0.5 litro", "500" },
                { "0.5 litros", "500" }, { "1/2litro", "500" }, { "1/2litros", "500" },
                { "500mililitros", "500" },
                { "600ml", "600" }, { "600 ml", "600" }, { "0.6lt", "600" },
                { "600mililitros", "600" }, { "600 mililitros", "600" },
                { "600 mil", "600" }, { "0.6l", "600" }, { "0.6 litros", "600" },
                { "250ml", "250" }, { "250 ml", "250" }, { "0.25lt", "250" },
                { "0.25l", "250" }, { "250mililitros", "250" }, { "250 mililitros", "250" },
                { "0.25 l", "250" }, { "250 mil", "250" },
                { "750ml", "750" }, { "750 ml", "750" }, { "0.75lt", "750" },
                { "0.75l", "750" }, { "750mililitros", "750" }, { "750 mililitros", "750" },
                { "330ml", "330" }, { "330 ml", "330" }, { "0.33lt", "330" },
                { "0.33l", "330" }, { "330mililitros", "330" }, { "330 mililitros", "330" },
                { "200ml", "200" }, { "200 ml", "200" }, { "0.2lt", "200" },
                { "0.2l", "200" }, { "200mililitros", "200" }, { "200 mililitros", "200" },
                { "150ml", "150" }, { "150 ml", "150" }, { "0.15lt", "150" },
                { "0.15l", "150" }, { "150mililitros", "150" }, { "150 mililitros", "150" },
                { "100ml", "100" }, { "100 ml", "100" }, { "0.1lt", "100" },
                { "0.1l", "100" }, { "100mililitros", "100" }, { "100 mililitros", "100" }
            };

            foreach (var equivalence in sizeEquivalences)
            {
                if (normalizedSize.Contains(equivalence.Key))
                {
                    return equivalence.Value;
                }
            }
            if (decimal.TryParse(Regex.Replace(normalizedSize, "[^0-9]", ""), out decimal sizeInMl))
            {
                return sizeInMl.ToString();
            }
            return normalizedSize;
        }


        private string NormalizeProductName(string name)
        {
            return Regex.Replace(name.Trim().ToLowerInvariant(), @"\s+", " ");
        }

        private bool IsLiquid(string size)
        {
            string lowerSize = size.ToLowerInvariant();
            return lowerSize.Contains("ml") || lowerSize.Contains("lt") || lowerSize.Contains("cl") ||
                   lowerSize.Contains("litro") || lowerSize.Contains("centrilitro") || lowerSize.Contains("mililitro") ||
                   lowerSize.Contains("l") || lowerSize.Contains("centrilitros") || lowerSize.Contains("litros") ||
                   lowerSize.Contains("mililitros");
        }

        private bool IsSolid(string size)
        {
            string lowerSize = size.ToLowerInvariant();
            return lowerSize.Contains("kg") || lowerSize.Contains("g") || lowerSize.Contains("gramos") ||
                   lowerSize.Contains("kilos") || lowerSize.Contains("kilogramo") || lowerSize.Contains("miligramos") ||
                   lowerSize.Contains("kilogramos") || lowerSize.Contains("mg");
        }


        private void DuplicateProductCheck(Product product)
        {
            ValidateProduct(product);
            var normalizedProductName = NormalizeString(product.Name);
            var normalizedProductSize = NormalizeString(product.Size);
            var similarProducts = _context.Products
                .AsEnumerable() 
                .Where(p => IsSimilar(NormalizeString(p.ProductName), normalizedProductName))
                .Select(p => new
                {
                    Name = p.ProductName,
                    Size = NormalizeString(p.ProductSize)
                })
                .ToList();

            if (similarProducts.Any(p => p.Size == normalizedProductSize))
            {
                throw new CustomException(
                    HttpStatusCode.Conflict,
                    "The product is already exists.",
                    "0004"
                );
            }
        }

        private string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return Regex.Replace(input.Trim().ToLower(), @"\s+", "");
        }

        private bool IsSimilar(string existingName, string newName, int threshold = 2)
        {
            if (existingName.Contains(newName) || newName.Contains(existingName))
                return true;

            return CalculateLevenshteinDistance(existingName, newName) <= threshold;
        }

        private int CalculateLevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return b.Length;
            if (string.IsNullOrEmpty(b)) return a.Length;

            var costs = new int[b.Length + 1];
            for (int j = 0; j < costs.Length; j++)
                costs[j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                costs[0] = i;
                int previousCost = i - 1;
                for (int j = 1; j <= b.Length; j++)
                {
                    int currentCost = costs[j];
                    costs[j] = Math.Min(
                        Math.Min(costs[j - 1] + 1, costs[j] + 1),
                        previousCost + (a[i - 1] == b[j - 1] ? 0 : 1));
                    previousCost = currentCost;
                }
            }
            return costs[b.Length];
        }

        private void ValidateProduct(Product product)
        {
            if (product == null)
                throw new CustomException(HttpStatusCode.BadRequest, "The product cannot be null.", "P001");
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new CustomException(HttpStatusCode.BadRequest, "The product name is required.", "P002");
            if (string.IsNullOrWhiteSpace(product.Size))
                throw new CustomException(HttpStatusCode.BadRequest, "The product size is required.", "P003");
        }

        #endregion
    }
}
