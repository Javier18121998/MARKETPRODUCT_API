using Market.Utilities.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Market.CustomersController
{
    public class CustomersController : MarketProductControllerBase<CustomersController>
    {
        public CustomersController(ILogger<CustomersController> logger)
            : base(logger)
        {

        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "",
            Description = ".")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succeded.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, ".")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, ".")]
        public async Task<ActionResult> GetAllProductsAsync()
        {
            return Ok();
        }
    }
}
