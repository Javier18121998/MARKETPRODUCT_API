using MARKETPRODUCT_API.Data.ModelValidations.IDataModelValidations;
using Microsoft.Extensions.Logging;

namespace MARKETPRODUCT_API.Data.ModelValidations
{
    public class ProductModelValidations : IProductModelValidations
    {
        private readonly ILogger<ProductModelValidations> _logger;

        public ProductModelValidations(ILogger<ProductModelValidations> logger)
        {
            _logger = logger;
        }

        public bool ValidateProductId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning($"Invalid product ID: {id}. It must be greater than 0.");
                return false; 
            }
            return true;
        }

        public bool ValidateProductName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("Product name is null or empty.");
                return false; 
            }
            return true;
        }

        public bool ValidateProductPrice(decimal price)
        {
            if (price <= 0)
            {
                _logger.LogWarning($"Invalid product price: {price}. It must be greater than 0.");
                return false; 
            }
            return true;
        }
    }
}
