using Market.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace Market.Utilities.BaseControllers
{
    /// <summary>
    /// Base class for all controllers in the Market Product API, providing common logging functionality.
    /// </summary>
    /// <typeparam name="TController">The type of the controller that derives from this base class.</typeparam>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MarketProductControllerBase<TController> : ControllerBase
    {
        protected readonly ILogger<TController> _logger;

        public MarketProductControllerBase(ILogger<TController> logger)
        {
            _logger = logger;
        }

        protected ActionResult HandleException(Exception e)
        {
            if (e is CustomException customEx)
            {
                var errorCode = customEx.ErrorCode ?? "UnknownErrorCode";
                var problemDetails = new
                {
                    Title = customEx.Message,
                    Detail = customEx.Message,
                    ErrorCode = errorCode
                };

                return Problem(
                    detail: System.Text.Json.JsonSerializer.Serialize(problemDetails),
                    statusCode: (int)customEx.StatusCode
                );
            }

            _logger.LogError($"An unexpected error occurred: {e.Message}");
            return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
}
