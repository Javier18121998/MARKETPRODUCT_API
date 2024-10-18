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

        public async Task DeleteProductByIdAsync(int id)
        {
            if (await _productValidationService.ProductExistsByIdAsync(id))
            {
                var producto = await _context.Products.FindAsync(id);
                _context.Products.Remove(producto);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("El producto con el ID especificado no existe.");
            }
        }

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
                throw new Exception("El producto con el nombre y tamaño especificado no existe.");
            }
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            return products;
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var prudctById =  await _context.Products.FindAsync(id);
            return prudctById;
        }

        public async Task<ProductDto> GetProductByNameAndSizeAsync(string name, string size)
        {
            var productByNameAndSize = await _context.Products.FirstOrDefaultAsync(
                p => p.ProductName == name && 
                p.ProductSize == size
            );
            return productByNameAndSize;
        }

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
                throw new Exception("El producto con el ID especificado no existe.");
            }
        }

        public async Task UpdateDescriptionByNameAndSizeAsync(string name, string size, string newDescription)
        {
            if (await _productValidationService.ProductExistsByNameAndSizeAsync(name, size))
            {
                var producto = await _context.Products.FirstOrDefaultAsync(
                    p => p.ProductName == name && 
                    p.ProductSize == size
                );
                producto.ProductDescription = newDescription;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("El producto con el nombre y tamaño especificado no existe.");
            }
        }
    }
}
