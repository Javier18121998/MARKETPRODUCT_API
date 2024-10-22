using Market.DAL.IDAL;
using Market.DataModels.EFModels;
using Market.DataValidation.IDataBaseValidations;
using Market.Market.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Market.DAL
{
    public class OrderValidationService : IOrderValidationService
    {
        private readonly MarketDbContext _context;

        public OrderValidationService(MarketDbContext context)
        {
            _context = context;
        }
        public async Task<bool> OrderExistsByIdAsync(int id)
        {
            return await _context.Orders.AnyAsync(o => o.Id == id);
        }
        public async Task<bool> OrderExistsByProductIdAsync(int productId)
        {
            return await _context.Orders.AnyAsync(o => o.ProductId == productId);
        }
        public async Task<bool> OrderExistsByProductNameAndSizeAsync(string productName, string productSize)
        {
            return await _context.Orders.AnyAsync(o => o.Product.ProductName == productName && o.Product.ProductSize == productSize);
        }
    }
}
