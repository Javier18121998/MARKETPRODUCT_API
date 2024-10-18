using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataValidation.IDataBaseValidations
{
    public interface IProductValidationService
    {
        Task<bool> ProductExistsByIdAsync(int id);
        Task<bool> ProductExistsByNameAndSizeAsync(string name, string size);
    }
}
