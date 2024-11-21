using Market.DataModels.EFModels;
using System.Threading.Tasks;

namespace Market.DAL.IDAL
{
    public interface ICartService
    {
        Task<Cart> AddItemToCartAsync(string productName, int quantity, string size);
        Task<Cart> GetCustomerCartAsync();
        Task<bool> RemoveItemFromCartAsync(string productName, string size);
    }
}
