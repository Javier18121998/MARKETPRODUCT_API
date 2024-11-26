using System;
using System.Linq;
using System.Net;
using System.Text;
using Market.BL.IBL;
using Market.DAL.IDAL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Market.Exceptions;
using Market.Utilities.MQServices.IManageServices;
using Market.Utilities.MQServices.IProduceServices;
using Market.Utilities.MQServices.ManageServices;

namespace Market.BL
{
    public class CustomerServiceBL : ICustomerServiceBL
    {
        private readonly ICustomerService _customerService;

        public CustomerServiceBL(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        public async Task<string> AuthenticateCustomerAsync(CustomerLogin login)
        {
            try
            {
                var tokenString = await _customerService.AuthenticateCustomerAsync(login);
                return tokenString;
            }
            catch (CustomException cex)
            {
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            try
            {
                var sessionValue = await _customerService.IsTokenValidAsync(token);
                return sessionValue;
            }
            catch (CustomException cex)
            {
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<Customer> RegisterCustomerAsync(CustomerRegistration registration)
        {
            try
            {
                var customer = await _customerService.RegisterCustomerAsync(registration);
                return customer;
            }
            catch (CustomException cex)
            {
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            try
            {
                var sessionRevoked = await _customerService.RevokeTokenAsync(token);
                return sessionRevoked;
            }
            catch (CustomException cex)
            {
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<CustomerDataRegistrationDto> CustomerDataRegistration(int customerId, CustomerDataRegistration customerDataRegistration)
        {
            try
            {
                var customerData = await _customerService.CustomerDataRegistration(customerId, customerDataRegistration);
                return customerData;
            }
            catch (CustomException cex)
            {
                throw new CustomException(HttpStatusCode.BadRequest, cex.Message, cex.ErrorCode);
            }
            catch (Exception)
            {
                throw new CustomException(HttpStatusCode.InternalServerError, "The Customer Id was not founded.");
            }
        }

        public async Task<CustomerDataRegistrationDto> GetCustomerDataAsync(int customerId)
        {
            try
            {
                return await _customerService.GetCustomerDataAsync(customerId);
            }
            catch (CustomException cex)
            {
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
            catch (Exception)
            {
                throw new CustomException(HttpStatusCode.InternalServerError, "Error retrieving customer data");
            }
        }

        public async Task<CustomerDataUpdateDto> UpdateCustomerDataAsync(int customerId, CustomerDataUpdate customerDataUpdate)
        {
            try
            {
                var customerUpdatedDto = await _customerService.UpdateCustomerDataAsync(customerId, customerDataUpdate);
                return customerUpdatedDto;
            }
            catch (CustomException cex)
            {
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
            catch (Exception)
            {
                throw new CustomException(HttpStatusCode.InternalServerError, "Error updating customer data");
            }
        }

        public async Task DeleteCustomerAsync(string tokenString)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(tokenString);
            }
            catch (CustomException cex)
            {
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
            catch (Exception)
            {
                throw new CustomException(HttpStatusCode.InternalServerError, "Error updating customer data");
            }
        }
    }
}