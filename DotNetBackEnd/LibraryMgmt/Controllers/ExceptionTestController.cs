using Microsoft.AspNetCore.Mvc;

namespace LibraryMgmt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExceptionTestController : ControllerBase
    {
        [HttpGet("Throw")]
        public IActionResult Throw()
        {
            throw new InvalidOperationException("Simulated exception");
        }
    }
}
