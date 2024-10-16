using MARKETPRODUCT_API.Data.EFModels;
using MARKETPRODUCT_API.Data;
using MARKETPRODUCT_API.Messaging.MessageModels;
using MARKETPRODUCT_API.Messaging.MessageProducer;
using MARKETPRODUCT_API.Services.IServices;

namespace MARKETPRODUCT_API.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductMQProducer _producerMQ;

        public ProductService(ApplicationDbContext context, ProductMQProducer producerMQ)
        {
            _context = context;
            _producerMQ = producerMQ;
        }

        public IEnumerable<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProduct(int id)
        {
            var product = _context.Products.Find(id);
            _producerMQ.SendMessage(new LogMessage { 
                Action = "Geting the products",
                ProductName = product.Name,
                ProductPrice = product.Price,
                Timestamp = DateTime.UtcNow
            });
            if (product == null) return null;
            return product;
        }

        public Product CreateProduct(Product newProduct)
        {
            _context.Products.Add(newProduct);
            _context.SaveChanges();

            var logMessage = new LogMessage
            {
                Action = "ProductAdded",
                ProductName = newProduct.Name,
                ProductPrice = newProduct.Price,
                Timestamp = DateTime.UtcNow
            };

            _producerMQ.SendMessage(logMessage);

            return newProduct;
        }

        public void UpdateProduct(int id, Product updatedProduct)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;

            _context.Products.Update(product);
            _context.SaveChanges();

            var logMessage = new LogMessage
            {
                Action = "ProductUpdated",
                ProductName = product.Name,
                ProductPrice = product.Price,
                Timestamp = DateTime.UtcNow
            };

            _producerMQ.SendMessage(logMessage);
        }

        public void DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            var logMessage = new LogMessage
            {
                Action = "ProductDeleted",
                ProductName = product.Name,
                ProductPrice = product.Price,
                Timestamp = DateTime.UtcNow
            };

            _producerMQ.SendMessage(logMessage);
        }
    }
}
