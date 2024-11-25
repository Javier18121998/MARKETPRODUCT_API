using Market.BL.IBL;
using Market.DataModels.EFModels;
using Market.Exceptions;
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
        private readonly ICustomerServiceBL _customerServiceBL;
        public CustomersController(ILogger<CustomersController> logger, ICustomerServiceBL customerServiceBL)
            : base(logger)
        {
            _customerServiceBL = customerServiceBL;
        }


        [Authorize]
        [HttpGet("validate-token")]
        public async Task<ActionResult> ValidateTokenAsync()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var isValid = await _customerServiceBL.IsTokenValidAsync(token);

            return isValid ? Ok(new { message = "Token is valid." }) : Unauthorized(new { message = "Token is invalid or expired." });
        }

        [HttpPost("CustomerRegistration")]
        [SwaggerOperation(
            Summary = "Obtiene los detalles de un Cliente",
            Description = "Obtine los deatalles de un cliente, mediante Su Email y Contraseña.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, ".")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, ".")]
        public async Task<ActionResult> CustomerRegistrationAsync([FromBody] CustomerRegistration registration)
        {
            var customer = await _customerServiceBL.RegisterCustomerAsync(registration);
            var token = await _customerServiceBL.AuthenticateCustomerAsync(new CustomerLogin
            {
                Email = registration.Email,
                Password = registration.Password
            });

            return Ok(new { customer, token });
        }

        [HttpPost("CustomerLoginAcces")]
        [SwaggerOperation(
            Summary = "Obtiene los detalles de un Cliente",
            Description = "Obtine los deatalles de un cliente, mediante Su Email y Contraseña.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, ".")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, ".")]
        public async Task<ActionResult> CustomerLoginAccesAsync([FromBody] CustomerLogin login)
        {
            var token = await _customerServiceBL.AuthenticateCustomerAsync(login);
            if (token == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("GetCustomerData")]
        [SwaggerOperation(Summary = "Get Customer Data", Description = "Retrieves customer data using the Customer ID from the JWT.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Customer data retrieved successfully.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal Server Error.")]
        public async Task<ActionResult> GetCustomerDataAsync()
        {
            try
            {
                var customerId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "customer_id")?.Value);
                if (customerId == 0)
                {
                    return BadRequest("Invalid customer ID.");
                }
                var customerData = await _customerServiceBL.GetCustomerDataAsync(customerId); // Call the appropriate method from the service layer
                if (customerData == null)
                {
                    return NotFound("Customer data not found.");
                }
                return Ok(new { customerData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer data.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [Authorize]
        [HttpPut("CustomerUpdateData")]
        [SwaggerOperation(
            Summary = "Update customer details.",
            Description = "Updates customer details using the Customer ID from the JWT and provided data.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Customer data updated successfully.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request.")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized.")] // Added Unauthorized response
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal Server Error.")]
        public async Task<ActionResult> CustomerUpdateDataAsync([FromBody] CustomerDataUpdate updateCustomerDto)
        {
            try
            {
                var customerId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "customer_id")?.Value);
                if (customerId == 0)
                {
                    return Unauthorized("Invalid customer ID."); // Return Unauthorized if ID is missing or invalid
                }

                // Update customer details
                await _customerServiceBL.UpdateCustomerDataAsync(customerId, updateCustomerDto);
                return Ok(new { message = "Customer data updated successfully." });
            }
            catch (CustomException cex)
            {
                _logger.LogError(cex, "Error updating customer data.");
                return StatusCode((int)cex.StatusCode, new { message = cex.Message, errorCode = cex.ErrorCode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer data.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [Authorize]
        [HttpDelete("DeleteCustomer")]
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

        [HttpPost("CustomerPasswordRecovery")]
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
        [HttpPost("CustomerLogout")]
        [SwaggerOperation(
            Summary = "Logout the CustomerSession",
            Description = "Close the session via jwt Token active revoke session.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "No enccount this session.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "somehting via jwt Token crash the error.")]
        public async Task<ActionResult> CustomerLogoutAsync()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _customerServiceBL.RevokeTokenAsync(token);
            if (!result)
            {
                return BadRequest(new { message = "Failed to logout." });
            }
            return Ok(new { message = "Successfully logged out." });
        }

        [Authorize]
        [HttpPost("PostingDataCustomer")]
        [SwaggerOperation(
            Summary = "Add Customer Data",
            Description = "Adds customer data using the Customer ID from the JWT.")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Customer data registered successfully.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal Server Error.")]
        public async Task<ActionResult> PostingDataCustomer([FromBody] CustomerDataRegistration customerDataRegistration)
        {
            try
            {
                var customerId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "customer_id")?.Value);
                if (customerId == 0)
                {
                    return BadRequest("Invalid customer ID.");
                }
                var customerData = await _customerServiceBL.CustomerDataRegistration(customerId, customerDataRegistration);
                return Ok(new { message = "Customer data registered successfully.", customerData });
            }
            catch (CustomException cex)
            {
                _logger.LogError(cex, "Error registering customer data.");
                return StatusCode((int)cex.StatusCode, new { message = cex.Message, errorCode = cex.ErrorCode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering customer data.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}