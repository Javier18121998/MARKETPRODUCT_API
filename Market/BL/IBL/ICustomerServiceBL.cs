using System;
using System.Linq;
using System.Text;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;

namespace Market.BL.IBL
{
    public interface ICustomerServiceBL
    {
        Task<Customer> RegisterCustomerAsync(CustomerRegistration registration);
        Task<string> AuthenticateCustomerAsync(CustomerLogin login);
        Task<bool> IsTokenValidAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<CustomerDataRegistrationDto> CustomerDataRegistration(int customerId, CustomerDataRegistration customerDataRegistration);
        Task<CustomerDataRegistrationDto> GetCustomerDataAsync(int customerId);
        Task<CustomerDataUpdateDto> UpdateCustomerDataAsync(int customerId, CustomerDataUpdate customerDataUpdate);
        Task DeleteCustomerAsync(string tokenString);
    }
}