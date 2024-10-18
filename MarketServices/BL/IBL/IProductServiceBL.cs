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
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<ProductDto> GetProductByNameAndSizeAsync(string name, string size);
        Task<ProductDto> CreateProductAsync(Product product);
        Task UpdateDescriptionByIdAsync(int id, string newDescription);
        Task UpdateDescriptionByNameAndSizeAsync(string name, string size, string newDescription);
        Task DeleteProductByIdAsync(int id);
        Task DeleteProductByNameAndSizeAsync(string name, string size);
    }
}
