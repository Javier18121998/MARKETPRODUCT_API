using Market.DAL.IDAL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Market.Market.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Market.DAL
{
    public class CustomerService : ICustomerService
    {
        private readonly MarketDbContext _context;
        private readonly JwtSettings _jwtSettings;

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
                    PasswordHash = registration.Password,
                    FullName = registration.FullName,
                    CreatedAt = dateTime
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> AuthenticateCustomerAsync(CustomerLogin login)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Email == login.Email && c.PasswordHash == HashPassword(login.Password));

            if (customer == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Email),
                new Claim("user_id", customer.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
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

        private string HashPassword(string password)
        {
            return password;
        }
    }
}
