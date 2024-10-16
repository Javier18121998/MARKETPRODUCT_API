using MARKETPRODUCT_API.Data.EFModels;

namespace MARKETPRODUCT_API.Services.IServices
{
    public interface IProductService
    {
        IEnumerable<Product> GetProducts();
        Product GetProduct(int id);
        Product CreateProduct(Product newProduct);
        void UpdateProduct(int id, Product updatedProduct);
        void DeleteProduct(int id);
    }
}
