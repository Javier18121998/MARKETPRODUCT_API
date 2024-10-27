﻿using Market.DAL.IDAL;
using Market.DataModels.EFModels;
using Market.Utilities.BaseControllers;
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
            return Ok(customer);
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
    }
}