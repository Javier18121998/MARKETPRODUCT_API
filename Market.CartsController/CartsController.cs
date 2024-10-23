using Market.Utilities.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Market.CartsController
{
    public class CartsController : MarketProductControllerBase<CartsController>
    {
        public CartsController(ILogger<CartsController> logger) 
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
            return null;
        }
    }
}
