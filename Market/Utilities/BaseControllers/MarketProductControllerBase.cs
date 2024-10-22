using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Utilities.BaseControllers
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
