using Market.DAL.IDAL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Market.DAL
{
    public class CustomerService : ICustomerService
    {
        private readonly List<Customer> _customers = new List<Customer>(); // This should be a database context in production
        private readonly JwtSettings _jwtSettings;

        public CustomerService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<Customer> RegisterCustomerAsync(CustomerRegistration registration)
        {
            var customer = new Customer
            {
                Id = _customers.Count + 1, // Simulando un ID
                Email = registration.Email,
                Password = HashPassword(registration.Password),
                FullName = registration.FullName
            };

            _customers.Add(customer); // Store in DB in production
            return await Task.FromResult(customer);
        }

        public async Task<string> AuthenticateCustomerAsync(CustomerLogin login)
        {
            var customer = _customers.FirstOrDefault(c => c.Email == login.Email && c.Password == HashPassword(login.Password));

            if (customer == null)
            {
                return null; // Invalid credentials
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, customer.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return await Task.FromResult(tokenHandler.WriteToken(token));
        }

        private string HashPassword(string password)
        {
            return password; // For demo purposes only
        }
    }
}
