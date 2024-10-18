using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataValidation.IDataBaseValidations
{
    public interface IOrderValidationService
    {
        Task<bool> OrderExistsByIdAsync(int id);
        Task<bool> OrderExistsByProductIdAsync(int productId);
        Task<bool> OrderExistsByProductNameAndSizeAsync(string productName, string productSize);
    }
}
