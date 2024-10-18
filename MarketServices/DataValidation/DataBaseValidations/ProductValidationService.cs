using Market.DataValidation.IDataBaseValidations;
using Market.Market.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataValidation.DataBaseValidations
{
    public class ProductValidationService : IProductValidationService
    {
        private readonly MarketDbContext _context;

        public ProductValidationService(MarketDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ProductExistsByIdAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }
        public async Task<bool> ProductExistsByNameAndSizeAsync(string name, string size)
        {
            return await _context.Products.AnyAsync(p => p.Name == name && p.Size == size);
        }
    }
}
