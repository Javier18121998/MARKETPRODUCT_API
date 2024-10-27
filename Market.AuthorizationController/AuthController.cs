using Market.AuthorizationController.AuthModels;
using Market.AuthorizationController.AuthServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MARKETPRODUCT_API.Controllers
{
    /// <summary>
    /// Controller responsible for handling authentication requests.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Initializes a new instance of the AuthController with the specified authentication service.
        /// </summary>
        /// <param name="authenticationService">The authentication service for handling user login.</param>
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Authenticates the user and generates a JWT token if credentials are valid.
        /// </summary>
        /// <param name="userLogin">The user login credentials.</param>
        /// <returns>An IActionResult containing the generated JWT token or an unauthorized response.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            var token = _authenticationService.Authenticate(userLogin.Username, userLogin.Password);
            if (token == null) return Unauthorized(new { message = "Invalid username or password" });

            return Ok(new { token });
        }
    }
}
