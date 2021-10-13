using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PoC.Api.Controllers
{
    [ApiController]
    [Route("api/logger")]
    public class LoggingController : ControllerBase
    {

        private readonly ILogger<LoggingController> _logger;

        public LoggingController(ILogger<LoggingController> logger)
        {
            _logger = logger;
        }

        [HttpGet, Route("info")]
        public IActionResult GetInformation()
        {
            _logger.LogInformation("This is some information {method}", "GetInformation");
            return NoContent();
        }

        [HttpGet, Route("error")]
        public IActionResult GetError()
        {
            try
            {
                var d = 0;
                var r = 1 / d;
                return NoContent();
            }
            catch (System.Exception e)
            {
                _logger.LogError("Cannot divide on zero", e);
                return BadRequest();
            }
        }
    }
}
