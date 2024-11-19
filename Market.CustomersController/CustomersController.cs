using Market.DAL.IDAL;
using Market.DataModels.EFModels;
using Market.Utilities.BaseControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Market.CustomersController
{
    [ApiVersion("2.0")]
    public class CustomersController : MarketProductControllerBase<CustomersController>
    {
        private readonly ICustomerService _customerService;
        public CustomersController(ILogger<CustomersController> logger, ICustomerService customerService)
            : base(logger)
        {
            _customerService = customerService;
        }


        [Authorize]
        [HttpGet("validate-token")]
        public async Task<ActionResult> ValidateTokenAsync()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var isValid = await _customerService.IsTokenValidAsync(token);

            return isValid ? Ok(new { message = "Token is valid." }) : Unauthorized(new { message = "Token is invalid or expired." });
        }

        [HttpPost("Register")]
        [SwaggerOperation(
            Summary = "Obtiene los detalles de un Cliente",
            Description = "Obtine los deatalles de un cliente, mediante Su Email y Contraseña.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, ".")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, ".")]
        public async Task<ActionResult> CustomerRegistratioAsync([FromBody] CustomerRegistration registration)
        {
            var customer = await _customerService.RegisterCustomerAsync(registration);
            var token = await _customerService.AuthenticateCustomerAsync(new CustomerLogin
            {
                Email = registration.Email,
                Password = registration.Password
            });

            return Ok(new { customer, token });
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Obtiene los detalles de un Cliente",
            Description = "Obtine los deatalles de un cliente, mediante Su Email y Contraseña.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, ".")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, ".")]
        public async Task<ActionResult> CustomerLoginAccesAsync([FromBody] CustomerLogin login)
        {
            var token = await _customerService.AuthenticateCustomerAsync(login);
            if (token == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            return Ok(new { token });
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Obtiene los detalles de un Cliente",
            Description = "Obtine los deatalles de un cliente, mediante Su Email y Contraseña.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, ".")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, ".")]
        public async Task<ActionResult> GetCustomerDataAsync()
        {
            return Ok();
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "Update all details of a customer.",
            Description = "Via mailId and password.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, ".")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, ".")]
        public async Task<ActionResult> CustomerUpdateDataAsync()
        {
            return Ok();
        }

        [HttpDelete]
        [SwaggerOperation(
            Summary = "Delete a customer into the customers table.",
            Description = "Delete a customer into the customers table, mediante Su Email y Contraseña.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, ".")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, ".")]
        public async Task<ActionResult> DeleteCustomerAsync()
        {
            return Ok();
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Recover the Password",
            Description = "Recover the Password via sending an email to his mailId and sending an auth secure key.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, ".")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, ".")]
        public async Task<ActionResult> CustomerPasswordRecoveryAsync()
        {
            return Ok();
        }

        [Authorize]
        [HttpPost("logout")]
        [SwaggerOperation(
            Summary = "Logout the CustomerSession",
            Description = "Close the session via jwt Token active revoke session.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "No enccount this session.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "somehting via jwt Token crash the error.")]
        public async Task<ActionResult> LogoutAsync()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _customerService.RevokeTokenAsync(token);
            if (!result)
            {
                return BadRequest(new { message = "Failed to logout." });
            }
            return Ok(new { message = "Successfully logged out." });
        }
    }
}