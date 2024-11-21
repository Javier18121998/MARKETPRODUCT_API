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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> AuthenticateCustomerAsync(CustomerLogin login)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Email == login.Email);
            string errorCode = string.Empty;

            if (customer == null || !VerifyPassword(login.Password, customer.PasswordHash))
            {
                errorCode = "MKPT00002";
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
