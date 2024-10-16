using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MARKETPRODUCT_API.Controllers.Utilities
{
    /// <summary>
    /// Base class for all controllers in the Market Product API, providing common logging functionality.
    /// </summary>
    /// <typeparam name="TController">The type of the controller that derives from this base class.</typeparam>
    [Route("api/[controller]")]
    [ApiController]
    public class MarketProductControllerBase<TController> : ControllerBase
    {
        protected readonly ILogger<TController> _logger;

        public MarketProductControllerBase(ILogger<TController> logger)
        {
            _logger = logger;
        }
    }
}
