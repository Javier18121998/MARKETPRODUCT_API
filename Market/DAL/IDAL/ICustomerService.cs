using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DAL.IDAL
{
    public interface ICustomerService
    {
        Task<Customer> RegisterCustomerAsync(CustomerRegistration registration);
        Task<string> AuthenticateCustomerAsync(CustomerLogin login);
        Task<bool> IsTokenValidAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<CustomerDataRegistrationDto> CustomerDataRegistration(int customerId, CustomerDataRegistration customerDataRegistration);
        Task<CustomerDataRegistrationDto> GetCustomerDataAsync(int customerId);
    }
}
