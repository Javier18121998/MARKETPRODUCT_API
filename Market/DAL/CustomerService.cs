using Market.DAL.IDAL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Market.Exceptions;
using Market.Market.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Market.DAL
{
    public class CustomerService : ICustomerService
    {
        private readonly MarketDbContext _context;
        private readonly JwtSettings _jwtSettings;
        public string errorCode { get; set; } = string.Empty;

        public CustomerService(IOptions<JwtSettings> jwtSettings, MarketDbContext context)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }

        public async Task<Customer> RegisterCustomerAsync(CustomerRegistration registration)
        {
            try
            {
                var dateTime = DateTime.UtcNow;
                var customer = new Customer
                {
                    Email = registration.Email,
                    PasswordHash = HashPassword(registration.Password),
                    FullName = registration.FullName,
                    CreatedAt = dateTime
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (CustomException)
            {
                errorCode = "MKPT00004";
                throw new CustomException(HttpStatusCode.BadRequest, "Fail to Register a new Customer", errorCode);
            }
        }

        public async Task<string> AuthenticateCustomerAsync(CustomerLogin login)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Email == login.Email);
            if (customer == null || !VerifyPassword(login.Password, customer.PasswordHash))
            {
                errorCode = "MKPT00004";
                throw new CustomException(HttpStatusCode.BadRequest, "Enmail or Password incorrect or maybe you don't have an account", errorCode);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, customer.Email),
                    new Claim("customer_id", customer.Id.ToString())
                ]),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var session = new CustomerSession
            {
                CustomerId = customer.Id,
                Token = tokenString,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes)
            };

            _context.CustomerSessions.Add(session);
            await _context.SaveChangesAsync();

            return tokenString;
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var session = await _context.CustomerSessions
                .FirstOrDefaultAsync(cs => cs.Token == token && cs.ExpiresAt > DateTime.UtcNow && !cs.IsRevoked);

            return session != null;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var session = await _context.CustomerSessions.FirstOrDefaultAsync(cs => cs.Token == token);

            if (session == null || session.IsRevoked)
            {
                return false;
            }

            session.IsRevoked = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CustomerDataRegistrationDto> CustomerDataRegistration(int customerId, CustomerDataRegistration customerDataRegistration)
        {
            try
            {
                var customerData = new CustomerDataRegistrationDto
                {
                    Id = customerId,
                    StreetAddress = customerDataRegistration.StreetAddress,
                    Neighborhood = customerDataRegistration.Neighborhood,
                    PostalCode = customerDataRegistration.PostalCode,
                    Country = customerDataRegistration.Country,
                    State = customerDataRegistration.State,
                    City = customerDataRegistration.City,
                    PhoneNumber = customerDataRegistration.PhoneNumber,
                    CURP = customerDataRegistration.UniquePopulationRegistryCode,
                    TaxId = customerDataRegistration.TaxId ?? string.Empty
                };
                _context.customerData.Add(customerData);
                await _context.SaveChangesAsync();
                return customerData;
            }
            catch (CustomException)
            {
                errorCode = "MKPT00004";
                throw new CustomException(HttpStatusCode.BadRequest, "Imposible action to register the customer data", errorCode);
            }
        }

        public async Task<CustomerDataRegistrationDto> GetCustomerDataAsync(int customerId)
        {
            var customerData = await _context.customerData
                .FirstOrDefaultAsync(cd => cd.Id == customerId);
            if (customerData == null)
            {
                errorCode = "MKPT00004";
                throw new CustomException(HttpStatusCode.BadRequest, "Customer data not found or Customer not exist.", errorCode);
            }
            return customerData;
        }

        public async Task<CustomerDataUpdateDto> UpdateCustomerDataAsync(int customerId, CustomerDataUpdate customerDataUpdate)
        {
            var customerData = await _context.customerData.FindAsync(customerId);
            if (customerData == null)
            {
                errorCode = "MKPT00003";
                throw new CustomException(HttpStatusCode.NotFound, "Customer data not found.", errorCode);
            }

            // Use reflection to update only the properties that have values in the DTO
            foreach (var property in typeof(CustomerDataUpdate).GetProperties()) // Reflect over CustomerDataUpdate
            {
                var newValue = property.GetValue(customerDataUpdate);
                if (newValue != null)
                {
                    var customerProperty = typeof(CustomerDataRegistrationDto).GetProperty(property.Name);
                    if (customerProperty != null && customerProperty.CanWrite) // Check if the property exists and is writable
                    {
                        try
                        {
                            customerProperty.SetValue(customerData, newValue);
                        }
                        catch (ArgumentException ex)
                        {
                            errorCode = "MKPT00004";
                            throw new CustomException(HttpStatusCode.BadRequest, $"Error updating {property.Name}: Type mismatch.", errorCode);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Create and return a DTO with updated values (map from customerData)
            var updatedDto = new CustomerDataUpdateDto
            {
                StreetAddress = customerData.StreetAddress,
                Neighborhood = customerData.Neighborhood,
                PostalCode = customerData.PostalCode,
                Country = customerData.Country,
                State = customerData.State,
                City = customerData.City,
                PhoneNumber = customerData.PhoneNumber
            };

            return updatedDto;
        }

        public async Task DeleteCustomerAsync(string tokenString)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync()) // Use a transaction for data integrity
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.ReadJwtToken(tokenString);
                    var customerIdClaim = token.Claims.FirstOrDefault(c => c.Type == "customer_id");
                    if (customerIdClaim == null)
                    {
                        errorCode = "MKPT00004";
                        throw new CustomException(HttpStatusCode.BadRequest, "The customer cannot be deleted or tha session has expired, please login again", errorCode);
                    }
                    var customerId = Convert.ToInt32(customerIdClaim.Value);
                    await DeletingSessionCustomerAsync(customerId);
                    await DeletingCustomerCart(customerId);
                    await DeletingCustomerDataAsync(customerId);
                    var customer = await _context.Customers.FindAsync(customerId);
                    if (customer == null)
                    {
                        throw new CustomException(HttpStatusCode.NotFound, "Customer not found.", "MKPT00003");
                    }
                    _context.Customers.Remove(customer);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (CustomException cex)
                {
                    await transaction.RollbackAsync();
                    throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
                }
            }
        }

        private async Task DeletingCustomerDataAsync(int customerId)
        {
            var customerData = await _context.customerData.FindAsync(customerId);
            if (customerData == null)
                return;
            _context.customerData.Remove(customerData);
            await _context.SaveChangesAsync();
        }

        private async Task DeletingSessionCustomerAsync(int customerId)
        {
            var customerSessions = await _context.CustomerSessions.Where(s => s.CustomerId == customerId).ToListAsync();
            if (customerSessions.Any())
            {
                _context.CustomerSessions.RemoveRange(customerSessions);
                await _context.SaveChangesAsync();
            }
        }

        private async Task DeletingCustomerCart(int customerId)
        {
            var cart = await _context.Cart.Include(c => c.Items).FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (cart != null)
            {
                _context.CartItem.RemoveRange(cart.Items); // Delete items first
                _context.Cart.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        private string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);

                byte[] hashBytes = new byte[16 + 32];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 32);

                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            byte[] storedPasswordHash = new byte[32];
            Array.Copy(hashBytes, 16, storedPasswordHash, 0, 32);

            using (var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);

                for (int i = 0; i < 32; i++)
                {
                    if (hash[i] != storedPasswordHash[i])
                        return false;
                }
            }

            return true;
        }
    }
}
