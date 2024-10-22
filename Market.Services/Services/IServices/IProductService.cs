using MARKETPRODUCT_API.Data.EFModels;
using System.Collections.Generic;
using System.Threading.Tasks; 

namespace MARKETPRODUCT_API.Services.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync(); 
        Task<Product> GetProductAsync(int id); 
        Task<Product> CreateProductAsync(Product newProduct);
        Task UpdateProductAsync(int id, Product updatedProduct);
        Task DeleteProductAsync(int id); 
    }
}
