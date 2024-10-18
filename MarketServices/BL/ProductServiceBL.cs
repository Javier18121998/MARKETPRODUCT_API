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
        public async Task<ProductDto> CreateProductAsync(Product product)
        {
            try
            {
                var productCreated =await _productService.CreateProductAsync(product);
                return productCreated;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public async Task DeleteProductByIdAsync(int id)
        {
            try
            {
                await _productService.DeleteProductByIdAsync(id);
            }
            catch (Exception ex)
            { 
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteProductByNameAndSizeAsync(string name, string size)
        {
            try
            {
                await _productService.DeleteProductByNameAndSizeAsync(name, size);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return products;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProductDto> GetProductByNameAndSizeAsync(string name, string size)
        {
            try
            {
                var productByNameAndSize = await _productService.GetProductByNameAndSizeAsync(name, size);
                return productByNameAndSize;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateDescriptionByIdAsync(int id, string newDescription)
        {
            try
            {
                await _productService.UpdateDescriptionByIdAsync(id, newDescription);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateDescriptionByNameAndSizeAsync(string name, string size, string newDescription)
        {
            try
            { 
                await _productService.UpdateDescriptionByNameAndSizeAsync(name, size, newDescription);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
