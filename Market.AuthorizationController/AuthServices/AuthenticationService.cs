using Market.AuthorizationController.AuthModels;
using Market.AuthorizationController.AuthServices.UserRoleServices.IUserServices;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Market.AuthorizationController.AuthServices
{
    /// <summary>
    /// Service responsible for handling authentication and generating JWT tokens.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserService _userService; // User service for user authentication handling

        /// <summary>
        /// Initializes the AuthenticationService with JWT settings and a user validation service.
        /// </summary>
        /// <param name="jwtSettings">JWT configuration settings.</param>
        /// <param name="userService">User service for validating user credentials.</param>
        public AuthenticationService(IOptions<JwtSettings> jwtSettings, IUserService userService)
        {
            _jwtSettings = jwtSettings.Value;
            _userService = userService;
        }

        /// <summary>
        /// Authenticates the user by validating credentials and generates a JWT token if valid.
        /// </summary>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The password for authentication.</param>
        /// <returns>A JWT token if authentication is successful, otherwise null.</returns>
        public string Authenticate(string username, string password)
        {
            // Validates user credentials using the user service
            var user = _userService.ValidateUser(username, password);
            if (user == null)
                throw new Exception("user name or password invalid.");  // Invalid username or password

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            // Define additional claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("role", user.Role), // User role
                new Claim("auth_id", user.Id.ToString()) // Unique user ID
            };

            // Configures the token with security parameters
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,  // Immediate token validity to avoid sync issues
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),  // Token expiration
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
