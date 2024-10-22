using MARKETPRODUCT_API.Data.EFModels;
using MARKETPRODUCT_API.Data;
using MARKETPRODUCT_API.Messaging.MessageModels;
using MARKETPRODUCT_API.Messaging.MessageProducer;
using MARKETPRODUCT_API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace MARKETPRODUCT_API.Services
{
    /// <summary>
    /// Purpose: Manage the Products on a store
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly MQProducer _MQProducer;

        public ProductService(ApplicationDbContext context, MQProducer mqProducer)
        {
            _context = context;
            _MQProducer = mqProducer;
        }

        /// <summary>
        /// Fetch all products on the wall
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductsAsync() 
        {
            return await _context.Products.ToListAsync(); 
        }

        /// <summary>
        /// Obtain every product by their Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Product> GetProductAsync(int id) 
        {
            var product = await _context.Products.FindAsync(id);
            _MQProducer.SendMessage(new LogMessage
            {
                Action = "Geting the products",
                ProductName = product?.Name,
                ProductPrice = product?.Price,
                Timestamp = DateTime.UtcNow
            });
            return product;
        }

        /// <summary>
        /// Create a Prodcut with: Id, Name, Price
        /// </summary>
        /// <param name="newProduct"></param>
        /// <returns></returns>
        public async Task<Product> CreateProductAsync(Product newProduct) 
        {
            await _context.Products.AddAsync(newProduct); 
            await _context.SaveChangesAsync(); 

            var logMessage = new LogMessage
            {
                Action = "ProductAdded",
                ProductName = newProduct.Name,
                ProductPrice = newProduct.Price,
                Timestamp = DateTime.UtcNow
            };

            _MQProducer.SendMessage(logMessage);

            return newProduct;
        }

        /// <summary>
        /// Updating in cost by their Id, and other elements of Product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedProduct"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpdateProductAsync(int id, Product updatedProduct) 
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;

            _context.Products.Update(product);
            await _context.SaveChangesAsync(); 

            var logMessage = new LogMessage
            {
                Action = "ProductUpdated",
                ProductName = product.Name,
                ProductPrice = product.Price,
                Timestamp = DateTime.UtcNow
            };

            _MQProducer.SendMessage(logMessage);
        }

        /// <summary>
        /// Delete the product with their Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteProductAsync(int id) 
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(); 

            var logMessage = new LogMessage
            {
                Action = "ProductDeleted",
                ProductName = product.Name,
                ProductPrice = product.Price,
                Timestamp = DateTime.UtcNow
            };

            _MQProducer.SendMessage(logMessage);
        }
    }
}
